using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kick_ChatBOT.Services
{
    public sealed class HeartbeatService : IDisposable
    {
        private readonly LicenseService _licenseService;
        private readonly string _key;
        private readonly string _hardwareId;
        private readonly TimeSpan _interval;
        private readonly TimeSpan _offlineGrace;
        private CancellationTokenSource _cts;
        private DateTime _lastOkUtc;

        public event Action<string> OnInvalid; // mensaje de error cuando la licencia deja de ser válida

        public HeartbeatService(LicenseService licenseService, string key, string hardwareId, TimeSpan interval, TimeSpan offlineGrace)
        {
            _licenseService = licenseService;
            _key = key;
            _hardwareId = hardwareId;
            _interval = interval;
            _offlineGrace = offlineGrace;
            _lastOkUtc = DateTime.UtcNow; // arranca en OK
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Task.Run(async () => await Loop(_cts.Token));
        }

        private async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var res = _licenseService.PingLicense(_key, _hardwareId);
                    if (res.Ok)
                    {
                        _lastOkUtc = DateTime.UtcNow;
                    }
                    else if (!res.ConnectionFailed)
                    {
                        OnInvalid?.Invoke(res.Message ?? "Licencia inválida");
                        break;
                    }
                    else
                    {
                        // Conexión fallida: evaluar gracia offline
                        if (DateTime.UtcNow - _lastOkUtc > _offlineGrace)
                        {
                            OnInvalid?.Invoke("Excedida gracia offline. Conéctate a internet para validar la licencia.");
                            break;
                        }
                    }
                }
                catch
                {
                    // ignorar errores del ciclo
                }

                await Task.Delay(_interval, token).ContinueWith(_ => { });
            }
        }

        public void Dispose()
        {
            try { _cts?.Cancel(); } catch { }
        }
    }
}


