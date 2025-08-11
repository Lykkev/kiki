using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kick_ChatBOT.Services
{
    // Cliente que usa curl.exe del sistema (como libcurl de Python)
    public class CurlApiClient : IApiClient
    {
        private readonly string proxyHost;
        private readonly int? proxyPort;
        private readonly string proxyUser;
        private readonly string proxyPass;

        public CurlApiClient(string proxyHost = null, int? proxyPort = null, string proxyUser = null, string proxyPass = null)
        {
            this.proxyHost = proxyHost;
            this.proxyPort = proxyPort;
            this.proxyUser = proxyUser;
            this.proxyPass = proxyPass;
        }

        private static string FindCurl()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Candidatos comunes (en orden de preferencia)
            var candidates = new[]
            {
                Path.Combine(baseDir, "curl.exe"),
                Path.Combine(baseDir, "curl", "curl.exe"),
                Path.Combine(baseDir, "curl-8.9.1_1-win64-mingw", "bin", "curl.exe"),
                Path.Combine(baseDir, "KICK CHATBOT", "curl-8.9.1_1-win64-mingw", "bin", "curl.exe")
            };

            foreach (var path in candidates)
            {
                if (File.Exists(path)) return path;
            }

            // Si no, confiar en el PATH del sistema (Windows 10/11 suelen tener curl)
            return "curl";
        }

        private async Task<Tuple<int, string, string>> RunCurlAsync(string args, CancellationToken ct)
        {
            var psi = new ProcessStartInfo
            {
                FileName = FindCurl(),
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            var p = new Process { StartInfo = psi, EnableRaisingEvents = true };
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();
            var tcs = new TaskCompletionSource<int>();

            p.OutputDataReceived += (s, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
            p.ErrorDataReceived += (s, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };
            p.Exited += (s, e) => tcs.TrySetResult(p.ExitCode);

            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            using (ct.Register(() => { try { if (!p.HasExited) p.Kill(); } catch { } tcs.TrySetCanceled(); }))
            {
                var code = await tcs.Task.ConfigureAwait(false);
                return Tuple.Create(code, stdout.ToString(), stderr.ToString());
            }
        }

        public async Task<Tuple<ChannelInfo, string>> GetChannelInfoAsync(string channelName, string bearerToken, CancellationToken ct)
        {
            var url = $"https://kick.com/api/v2/channels/{channelName}";
            var headers = new StringBuilder();
            headers.Append(" -H \"Accept: application/json\"");
            if (!string.IsNullOrEmpty(bearerToken))
            {
                headers.Append($" -H \"Authorization: Bearer {bearerToken}\"");
            }
            headers.Append(" -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36\"");

            var proxyArgs = BuildProxyArgs();
            var args = $"-s -i{headers}{proxyArgs} \"{url}\""; // -i para incluir headers de respuesta
            var result = await RunCurlAsync(args, ct).ConfigureAwait(false);
            var exit = result.Item1; var output = result.Item2; var error = result.Item3;

            // Separar headers y body
            var split = output.Replace("\r\n", "\n");
            var idx = split.IndexOf("\n\n");
            var body = idx >= 0 ? split.Substring(idx + 2) : split;

            if (exit == 0)
            {
                try
                {
                    var channel = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelInfo>(body);
                    if (channel != null) return Tuple.Create(channel, (string)null);
                }
                catch { }
            }
            return Tuple.Create<ChannelInfo, string>(null, $"cURL fallo (exit {exit}): {TrimForLog(error)} | Body: {TrimForLog(body)}");
        }

        public async Task<Tuple<bool, int, string>> SendMessageAsync(long chatroomId, string message, string bearerToken, CancellationToken ct)
        {
            var url = $"https://kick.com/api/v2/messages/send/{chatroomId}";
            var payload = $"{{\"content\":\"{EscapeJson(message)}\",\"type\":\"message\"}}";

            var headers = new StringBuilder();
            headers.Append(" -H \"Accept: application/json\"");
            headers.Append(" -H \"Content-Type: application/json\"");
            headers.Append(" -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36\"");
            if (!string.IsNullOrEmpty(bearerToken))
            {
                headers.Append($" -H \"Authorization: Bearer {bearerToken}\"");
            }

            var proxyArgs = BuildProxyArgs();
            var args = $"-s -i -X POST{headers}{proxyArgs} --data {EscapeArg(payload)} \"{url}\"";
            var result = await RunCurlAsync(args, ct).ConfigureAwait(false);
            var exit = result.Item1; var output = result.Item2; var error = result.Item3;

            // Extraer status code del primer header HTTP/1.1 200 OK
            int status = -1;
            foreach (var line in output.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None))
            {
                if (line.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = line.Split(' ');
                    if (parts.Length >= 2 && int.TryParse(parts[1], out var s)) { status = s; break; }
                }
            }

            var bodyIdx = output.IndexOf("\r\n\r\n");
            if (bodyIdx < 0) bodyIdx = output.IndexOf("\n\n");
            var body = bodyIdx >= 0 ? output.Substring(bodyIdx + 4) : output;

            if (exit == 0 && status >= 200 && status < 300)
            {
                return Tuple.Create(true, status, body);
            }
            return Tuple.Create(false, status <= 0 ? exit : status, string.IsNullOrWhiteSpace(body) ? TrimForLog(error) : body);
        }

        private static string EscapeArg(string s)
        {
            if (s == null) return "\"\"";
            return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }

        private static string EscapeJson(string s)
        {
            return s?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? string.Empty;
        }

        private static string TrimForLog(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace("\r", " ").Replace("\n", " ");
            return s.Length > 300 ? s.Substring(0, 300) + "..." : s;
        }

        public void Dispose()
        {
            // Nada que liberar
        }

        private string BuildProxyArgs()
        {
            if (string.IsNullOrWhiteSpace(proxyHost) || proxyPort == null) return string.Empty;
            var args = $" --proxy http://{proxyHost}:{proxyPort.Value}";
            if (!string.IsNullOrEmpty(proxyUser))
            {
                args += $" --proxy-user {EscapeArg(proxyUser + ":" + (proxyPass ?? string.Empty))}";
            }
            return args;
        }
    }
}


