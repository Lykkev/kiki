using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kick_ChatBOT.Services
{
    public static class SecureLocalCache
    {
        private static string CachePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KickChatBotWin", "auth.dat");
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("kcb-salt-2025");

        public static void SaveBoundKey(string key, string hardwareId)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CachePath));
            var data = Encoding.UTF8.GetBytes(key + "|" + hardwareId);
            var enc = Protect(data);
            File.WriteAllBytes(CachePath, enc);
        }

        public static Tuple<string, string> LoadBoundKey()
        {
            if (!File.Exists(CachePath)) return null;
            var enc = File.ReadAllBytes(CachePath);
            var dec = Unprotect(enc);
            var s = Encoding.UTF8.GetString(dec);
            var parts = s.Split('|');
            if (parts.Length != 2) return null;
            return Tuple.Create(parts[0], parts[1]);
        }

        private static byte[] Protect(byte[] data)
        {
            try
            {
                return ProtectedData.Protect(data, Salt, DataProtectionScope.CurrentUser);
            }
            catch
            {
                // Fallback Aes
                using (var aes = Aes.Create())
                {
                    aes.Key = DeriveKey();
                    aes.IV = new byte[16];
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        private static byte[] Unprotect(byte[] enc)
        {
            try
            {
                return ProtectedData.Unprotect(enc, Salt, DataProtectionScope.CurrentUser);
            }
            catch
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = DeriveKey();
                    aes.IV = new byte[16];
                    using (var ms = new MemoryStream(enc))
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var outMs = new MemoryStream())
                    {
                        cs.CopyTo(outMs);
                        return outMs.ToArray();
                    }
                }
            }
        }

        private static byte[] DeriveKey()
        {
            var hw = LicenseService.ComputeHardwareId();
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(hw + "|kcb-local"));
            }
        }
    }
}


