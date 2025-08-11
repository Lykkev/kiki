using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kick_ChatBOT.Models;
using Newtonsoft.Json;

namespace Kick_ChatBOT.Services
{
    public class KickApiClient : IDisposable, IApiClient
    {
        private readonly HttpClient httpClient;

        public KickApiClient(IWebProxy proxy = null)
        {
            var handler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = proxy != null,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false  // Desactivar cookies para evitar problemas
            };
            
            // Log información del proxy
            if (proxy != null)
            {
                System.Diagnostics.Debug.WriteLine($"HttpClient configurado CON proxy: {proxy.GetProxy(new Uri("https://kick.com"))}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"HttpClient configurado SIN proxy");
            }
            
            httpClient = new HttpClient(handler);
            // NO configurar User-Agent por defecto - se agregará por request
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<Tuple<ChannelInfo, string>> GetChannelInfoAsync(string channelName, string bearerToken, CancellationToken ct)
        {
            try
            {
                var url = $"https://kick.com/api/v2/channels/{channelName}";
                System.Diagnostics.Debug.WriteLine($"=== OBTENIENDO INFO DEL CANAL ===");
                System.Diagnostics.Debug.WriteLine($"URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Token: {bearerToken?.Substring(0, Math.Min(30, bearerToken?.Length ?? 0))}...");
                
                using (var req = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // REPLICANDO EXACTAMENTE LOS HEADERS DE PYTHON - PERO MÁS COMPLETO
                    req.Headers.Add("Accept", "application/json");
                    req.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                    
                    // IMPORTANTE: Solo agregar Authorization si realmente tenemos token
                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        req.Headers.Add("Authorization", $"Bearer {bearerToken}");
                        System.Diagnostics.Debug.WriteLine($"Authorization header agregado (MODO PYTHON)");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Sin token - request sin Authorization (MODO PYTHON)");
                    }

                    // Log todos los headers que se van a enviar
                    System.Diagnostics.Debug.WriteLine($"Headers que se enviarán (MODO PYTHON):");
                    foreach (var header in req.Headers)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }

                    var resp = await httpClient.SendAsync(req, ct).ConfigureAwait(false);
                    var status = (int)resp.StatusCode;
                    var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

                    System.Diagnostics.Debug.WriteLine($"=== RESPUESTA GetChannelInfo ===");
                    System.Diagnostics.Debug.WriteLine($"Status: {status} ({resp.StatusCode})");
                    System.Diagnostics.Debug.WriteLine($"Body length: {body?.Length ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"Body: '{body}'");
                    
                    // Log headers de respuesta
                    System.Diagnostics.Debug.WriteLine($"Response headers:");
                    foreach (var header in resp.Headers)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }
                    if (resp.Content.Headers != null)
                    {
                        foreach (var header in resp.Content.Headers)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                        }
                    }

                    if (resp.IsSuccessStatusCode)
                    {
                        var channelInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelInfo>(body);
                        System.Diagnostics.Debug.WriteLine($"Canal ID: {channelInfo?.Id}, Chatroom ID: {channelInfo?.ChatRoom?.Id}");
                        return Tuple.Create(channelInfo, (string)null);
                    }
                    return Tuple.Create<ChannelInfo, string>(null, $"Error {status}: {body}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPCIÓN EN GetChannelInfoAsync: {ex}");
                return Tuple.Create<ChannelInfo, string>(null, $"Excepción: {ex.Message}");
            }
        }

        public async Task<Tuple<bool, int, string>> SendMessageAsync(long chatroomId, string message, string bearerToken, CancellationToken ct)
        {
            try
            {
                var url = $"https://kick.com/api/v2/messages/send/{chatroomId}";
                
                // LOG DETALLADO ANTES DE ENVIAR
                System.Diagnostics.Debug.WriteLine($"=== ENVIANDO MENSAJE ===");
                System.Diagnostics.Debug.WriteLine($"URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Token: {bearerToken?.Substring(0, Math.Min(30, bearerToken?.Length ?? 0))}...");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {message}");
                
                using (var req = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    // REPLICANDO EXACTAMENTE LOS HEADERS DE PYTHON PARA SEND_MESSAGE
                    req.Headers.Add("Accept", "application/json");
                    req.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                    
                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        req.Headers.Add("Authorization", $"Bearer {bearerToken}");
                    }
                    
                    var payload = "{\"content\":\"" + EscapeJson(message) + "\",\"type\":\"message\"}";
                    req.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    
                    // IMPORTANTE: Python también tiene content-type en los headers
                    // Ya se agrega automáticamente con StringContent, pero lo mencionamos
                    
                    // LOG HEADERS Y PAYLOAD
                    System.Diagnostics.Debug.WriteLine($"Headers enviados:");
                    foreach (var header in req.Headers)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }
                    if (req.Content.Headers != null)
                    {
                        foreach (var header in req.Content.Headers)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"Payload: {payload}");

                    var resp = await httpClient.SendAsync(req, ct).ConfigureAwait(false);
                    var status = (int)resp.StatusCode;
                    var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    
                    // LOG RESPUESTA DETALLADA
                    System.Diagnostics.Debug.WriteLine($"=== RESPUESTA RECIBIDA ===");
                    System.Diagnostics.Debug.WriteLine($"Status: {status} ({resp.StatusCode})");
                    System.Diagnostics.Debug.WriteLine($"Headers de respuesta:");
                    foreach (var header in resp.Headers)
                    {
                        System.Diagnostics.Debug.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                    }
                    System.Diagnostics.Debug.WriteLine($"Body: {body}");
                    System.Diagnostics.Debug.WriteLine($"========================");
                    
                    if (resp.IsSuccessStatusCode)
                    {
                        return Tuple.Create(true, status, body);
                    }
                    return Tuple.Create(false, status, body);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPCIÓN EN SendMessageAsync: {ex}");
                return Tuple.Create(false, -1, ex.Message);
            }
        }

        private static string EscapeJson(string s)
        {
            return s?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? string.Empty;
        }

        // Método simple para probar conectividad con Kick.com
        public async Task<Tuple<bool, string>> TestConnectivityAsync(CancellationToken ct)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== PROBANDO CONECTIVIDAD BÁSICA ===");
                
                // Test 1: Solo headers básicos
                using (var req = new HttpRequestMessage(HttpMethod.Get, "https://kick.com/api/v2/channels/nhyrkal"))
                {
                    // SOLO headers básicos para evitar detección
                    req.Headers.Add("Accept", "application/json");

                    System.Diagnostics.Debug.WriteLine($"Test básico a: {req.RequestUri}");

                    var resp = await httpClient.SendAsync(req, ct).ConfigureAwait(false);
                    var status = (int)resp.StatusCode;
                    var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

                    System.Diagnostics.Debug.WriteLine($"Test básico Status: {status}");
                    System.Diagnostics.Debug.WriteLine($"Test básico Body length: {body?.Length ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"Test básico Body preview: {(body?.Length > 50 ? body.Substring(0, 50) + "..." : body)}");

                    if (resp.IsSuccessStatusCode)
                    {
                        return Tuple.Create(true, $"Conectividad OK (básica) - Status {status}, Response length: {body?.Length ?? 0}");
                    }
                }
                
                // Test 2: Probar página principal de Kick.com
                System.Diagnostics.Debug.WriteLine("=== PROBANDO PÁGINA PRINCIPAL ===");
                using (var req2 = new HttpRequestMessage(HttpMethod.Get, "https://kick.com/"))
                {
                    // Con User-Agent completo como navegador
                    req2.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                    
                    var resp2 = await httpClient.SendAsync(req2, ct).ConfigureAwait(false);
                    var status2 = (int)resp2.StatusCode;
                    var body2 = await resp2.Content.ReadAsStringAsync().ConfigureAwait(false);

                    System.Diagnostics.Debug.WriteLine($"Página principal Status: {status2}");
                    System.Diagnostics.Debug.WriteLine($"Página principal Body length: {body2?.Length ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"Página principal Body preview: {(body2?.Length > 100 ? body2.Substring(0, 100) + "..." : body2)}");
                    
                    if (resp2.IsSuccessStatusCode)
                    {
                        return Tuple.Create(true, $"Kick.com accesible - Status {status2}, puede ser bloqueo de API específico");
                    }
                    else
                    {
                        return Tuple.Create(false, $"Kick.com completamente bloqueado - Status {status2}");
                    }
                }
                
                // Test 3: API sin Authorization header para comparar
                System.Diagnostics.Debug.WriteLine("=== PROBANDO API SIN TOKEN ===");
                using (var req3 = new HttpRequestMessage(HttpMethod.Get, "https://kick.com/api/v2/channels/nhyrkal"))
                {
                    req3.Headers.Add("Accept", "application/json");
                    req3.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                    
                    var resp3 = await httpClient.SendAsync(req3, ct).ConfigureAwait(false);
                    var status3 = (int)resp3.StatusCode;
                    var body3 = await resp3.Content.ReadAsStringAsync().ConfigureAwait(false);

                    System.Diagnostics.Debug.WriteLine($"API sin token Status: {status3}");
                    System.Diagnostics.Debug.WriteLine($"API sin token Body: {body3}");
                    
                    if (resp3.IsSuccessStatusCode)
                    {
                        return Tuple.Create(true, $"API accesible sin token - problema específico de Authorization");
                    }
                    else if (status3 == 401)
                    {
                        return Tuple.Create(true, $"API accesible (401 esperado sin token) - problema puede ser token específico");
                    }
                    else
                    {
                        return Tuple.Create(false, $"API también bloqueada sin token - Status {status3}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error de conectividad: {ex}");
                return Tuple.Create(false, $"Excepción de conectividad: {ex.Message}");
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }

    public class ChannelInfo
    {
        [JsonProperty("id")] public long Id { get; set; }
        [JsonProperty("chatroom")] public ChatRoom ChatRoom { get; set; }
        [JsonProperty("livestream")] public LiveStream LiveStream { get; set; }
        [JsonProperty("recent_categories")] public List<Category> RecentCategories { get; set; }
    }

    public class ChatRoom
    {
        [JsonProperty("id")] public long Id { get; set; }
    }

    public class LiveStream
    {
        [JsonProperty("category")] public Category Category { get; set; }
        [JsonProperty("categories")] public List<Category> Categories { get; set; }
    }

    public class Category
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("slug")] public string Slug { get; set; }
    }
}

