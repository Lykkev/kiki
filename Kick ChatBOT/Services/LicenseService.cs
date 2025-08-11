using System;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kick_ChatBOT.Services
{
    public enum LicenseType
    {
        Trial3Days,
        Trial7Days,
        Days30,
        Lifetime
    }

    public sealed class LicenseInfo
    {
        public ObjectId Id { get; set; }
        public string Key { get; set; }
        public string OwnerEmail { get; set; }
        public string HardwareId { get; set; }
        public LicenseType Type { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public bool Revoked { get; set; }
        public int Activations { get; set; }
        public string Notes { get; set; }
        public DateTime? LastSeenUtc { get; set; }
    }

    public sealed class LicenseService
    {
        private readonly IMongoCollection<LicenseInfo> _collection;
        private readonly string _aesKey;

        public LicenseService(string mongoConnection, string database = "kickchatbot", string collection = "licenses", string aesKey = null)
        {
            var client = new MongoClient(mongoConnection);
            var db = client.GetDatabase(database);
            _collection = db.GetCollection<LicenseInfo>(collection);
            _aesKey = aesKey ?? "kcb-secure-key-2025"; // reemplazar en deployment
        }

        public static string ComputeHardwareId()
        {
            // Combinar varios identificadores del sistema para un fingerprint estable
            string cpu = Wmi("Win32_Processor", "ProcessorId");
            string bios = Wmi("Win32_BIOS", "SerialNumber");
            string board = Wmi("Win32_BaseBoard", "SerialNumber");
            string disk = Wmi("Win32_DiskDrive", "SerialNumber");
            string os = Wmi("Win32_OperatingSystem", "SerialNumber");

            var raw = string.Join("|", new[] { cpu, bios, board, disk, os }.Select(x => x?.Trim() ?? "-"));
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }

        private static string Wmi(string cls, string prop)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher($"select {prop} from {cls}"))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mo = (ManagementObject)o;
                        return mo[prop]?.ToString();
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        public LicenseInfo ValidateAndBind(string licenseKey)
        {
            var hw = ComputeHardwareId();

            LicenseInfo lic;
            try
            {
                lic = _collection.Find(x => x.Key == licenseKey).FirstOrDefault();
            }
            catch (TimeoutException)
            {
                throw; // se maneja arriba
            }
            catch (Exception ex)
            {
                // Cualquier error de cliente/servidor Mongo se reexpone como InvalidOperation para que la UI lo maneje sin ambigüedad de tipos
                throw new InvalidOperationException("No se pudo conectar al servidor de licencias: " + ex.Message);
            }

            if (lic == null) throw new InvalidOperationException("Licencia no encontrada o inválida");
            if (lic.Revoked) throw new InvalidOperationException("Licencia revocada");
            if (lic.ExpiresAtUtc.HasValue && DateTime.UtcNow > lic.ExpiresAtUtc.Value)
                throw new InvalidOperationException("Licencia expirada");

            // Primera activación: enlazar al hardware
            if (string.IsNullOrEmpty(lic.HardwareId))
            {
                var update = Builders<LicenseInfo>.Update
                    .Set(x => x.HardwareId, hw)
                    .Inc(x => x.Activations, 1);
                _collection.UpdateOne(x => x.Id == lic.Id, update);
                lic.HardwareId = hw;
                lic.Activations += 1;
            }
            else if (!string.Equals(lic.HardwareId, hw, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Esta licencia ya está activada en otro equipo");
            }

            return lic;
        }

        public bool TryValidateAndBind(string licenseKey, out LicenseInfo license, out string error)
        {
            license = null;
            error = null;
            try
            {
                var lic = ValidateAndBind(licenseKey);
                license = lic;
                return true;
            }
            catch (TimeoutException tex)
            {
                error = "Tiempo de espera al conectar con el servidor de licencias: " + tex.Message;
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                error = ioe.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = "Error inesperado de licencias: " + ex.Message;
                return false;
            }
        }

        public LicenseInfo CreateLicense(LicenseType type, string ownerEmail = null, string notes = null)
        {
            var key = GenerateKey();
            var now = DateTime.UtcNow;
            DateTime? exp = null;
            switch (type)
            {
                case LicenseType.Trial3Days: exp = now.AddDays(3); break;
                case LicenseType.Trial7Days: exp = now.AddDays(7); break;
                case LicenseType.Days30: exp = now.AddDays(30); break;
                case LicenseType.Lifetime: exp = null; break;
            }

            var lic = new LicenseInfo
            {
                Key = key,
                OwnerEmail = string.IsNullOrWhiteSpace(ownerEmail) ? null : ownerEmail,
                HardwareId = null,
                Type = type,
                CreatedAtUtc = now,
                ExpiresAtUtc = exp,
                Revoked = false,
                Activations = 0,
                Notes = notes
            };

            _collection.InsertOne(lic);
            return lic;
        }

        private string GenerateKey()
        {
            // Formato: KCB-XXXX-XXXX-XXXX-XXXX
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buf = new byte[16];
                rng.GetBytes(buf);
                string chunk(int i) => BitConverter.ToString(buf.Skip(i).Take(4).ToArray()).Replace("-", "");
                return $"KCB-{chunk(0)}-{chunk(4)}-{chunk(8)}-{chunk(12)}";
            }
        }

        public sealed class PingResult
        {
            public bool Ok { get; set; }
            public bool ConnectionFailed { get; set; }
            public string Message { get; set; }
            public DateTime? ExpiresAtUtc { get; set; }
        }

        public PingResult PingLicense(string key, string hardwareId)
        {
            try
            {
                var lic = _collection.Find(x => x.Key == key).FirstOrDefault();
                if (lic == null)
                {
                    return new PingResult { Ok = false, Message = "Licencia no encontrada" };
                }
                if (lic.Revoked)
                {
                    return new PingResult { Ok = false, Message = "Licencia revocada" };
                }
                if (lic.ExpiresAtUtc.HasValue && DateTime.UtcNow > lic.ExpiresAtUtc.Value)
                {
                    return new PingResult { Ok = false, Message = "Licencia expirada", ExpiresAtUtc = lic.ExpiresAtUtc };
                }
                if (!string.IsNullOrEmpty(lic.HardwareId) && !string.Equals(lic.HardwareId, hardwareId, StringComparison.OrdinalIgnoreCase))
                {
                    return new PingResult { Ok = false, Message = "La licencia está activada en otro equipo" };
                }

                // Actualizar last seen
                var update = Builders<LicenseInfo>.Update.Set(x => x.LastSeenUtc, DateTime.UtcNow);
                _collection.UpdateOne(x => x.Id == lic.Id, update);
                return new PingResult { Ok = true, ExpiresAtUtc = lic.ExpiresAtUtc };
            }
            catch (TimeoutException)
            {
                return new PingResult { Ok = false, ConnectionFailed = true, Message = "Timeout conectando a licencias" };
            }
            catch (Exception ex)
            {
                return new PingResult { Ok = false, ConnectionFailed = true, Message = ex.Message };
            }
        }
    }
}


