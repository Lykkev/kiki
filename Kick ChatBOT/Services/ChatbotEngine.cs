using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kick_ChatBOT.Models;

namespace Kick_ChatBOT.Services
{
    public class ChatbotEngine
    {
        private readonly string basePath;
        private readonly Random random;

        public List<Account> Accounts { get; private set; }
        public AppConfig Config { get; private set; }
        public Messages Messages { get; private set; }
        public ProxiesFile Proxies { get; private set; }
        public bool UseProxies { get; set; }
        public string KicksPath { get; set; }
        public string ConfigPath { get; set; }
        public string MessagesPath { get; set; }
        public string ProxiesPath { get; set; }

        public ChatbotEngine(string basePath)
        {
            this.basePath = basePath;
            this.random = new Random();
        }

        /// <summary>
        /// NUEVA LÓGICA: Solo usar archivo seleccionado por usuario
        /// Si no hay archivo seleccionado, devolver null y usar defaults en memoria
        /// NUNCA usar los archivos basura de bin/Debug
        /// </summary>
        private string GetPriorityPath(string customPath, string defaultFileName)
        {
            // SOLO usar ruta personalizada si existe
            if (!string.IsNullOrEmpty(customPath) && File.Exists(customPath))
            {
                return customPath;
            }

            // Si no hay archivo del usuario, NO usar ningún archivo
            return null; // Retornar null para usar defaults
        }

        public void LoadAll()
        {
            // SOLO usar archivos del usuario, NUNCA los de bin/Debug
            var kicksPath = GetPriorityPath(KicksPath, "kicks.json");
            var configPath = GetPriorityPath(ConfigPath, "config.json");
            var messagesPath = GetPriorityPath(MessagesPath, "messages.json");
            var proxiesPath = GetPriorityPath(ProxiesPath, "proxies.json");

            // Cargar cuentas: SOLO del archivo del usuario
            if (kicksPath != null)
            {
                Accounts = JsonStorage.ReadJsonFile(kicksPath, new List<Account>());
            }
            else
            {
                Accounts = new List<Account>();
            }

            // Cargar configuración
            if (configPath != null)
            {
                Config = JsonStorage.ReadJsonFile(configPath, GetDefaultConfig());
            }
            else
            {
                Config = GetDefaultConfig();
            }

            // Cargar mensajes
            if (messagesPath != null)
            {
                Messages = JsonStorage.ReadJsonFile(messagesPath, GetDefaultMessages());
            }
            else
            {
                Messages = GetDefaultMessages();
            }

            // Cargar proxies
            if (proxiesPath != null)
            {
                Proxies = JsonStorage.ReadJsonFile(proxiesPath, GetDefaultProxies());
            }
            else
            {
                Proxies = GetDefaultProxies();
            }

            UseProxies = Proxies?.Settings?.ProxyEnabled ?? false;
        }

        private AppConfig GetDefaultConfig()
        {
            return new AppConfig
            {
                Delays = new Delays
                {
                    InitialWait = new DelayRange { Min = 10, Max = 30 },
                    BetweenMessages = new DelayRange { Min = 180, Max = 300 },
                    TypingSimulation = new DelayRange { Min = 1, Max = 4 },
                    BetweenAccounts = 5,
                    BatchPause = new DelayRange { Min = 300, Max = 600 }
                },
                Behavior = new Behavior
                {
                    MaxConcurrentBots = 3,
                    MessagesPerSession = 5,
                    AddHumanVariations = true,
                    RandomizeOrder = true,
                    OnlyEmotes = false
                },
                Safety = new Safety
                {
                    RespectRateLimits = true,
                    AutoRetryOn429 = true,
                    MaxRetriesPerAccount = 3,
                    StopOnTokenError = true
                },
                Proxy = new ProxySettings
                {
                    Enabled = false,
                    AutoAssign = true,
                    ShowProxyInfo = true,
                    FallbackToDirect = false,
                    MaxRetriesPerProxy = 3
                }
            };
        }

        private Messages GetDefaultMessages()
        {
            return new Messages
            {
                Personalities = new Dictionary<string, List<string>>
                {
                    { "casual", new List<string> { "nice stream! 😄", "gg wp 👏", "lol 😂", "good play 🎮" } },
                    { "enthusiastic", new List<string> { "AMAZING! 🤩", "SO GOOD! 🔥", "POGGERS! 😍" } },
                    { "supportive", new List<string> { "great job! 👏", "you got this! 💪", "keep it up! ⭐" } },
                    { "gamer", new List<string> { "ez clap 😎", "pogchamp 🎮", "5head play 🧠" } },
                    { "lurker", new List<string> { "👀", "nice 👍", "gg", "wp" } },
                    { "toxic", new List<string> { "noob 🙄", "ez game", "git gud" } }
                },
                KickEmotes = new Dictionary<string, List<string>>
                {
                    { "hype", new List<string> { "[emote:37248:ratJAM]", "[emote:39268:HYPERCLAPH]", "[emote:39284:vibePlz]" } },
                    { "chill", new List<string> { "[emote:1730752:emojiAngel]", "[emote:39273:MuteD]" } }
                },
                Emojis = new Dictionary<string, List<string>>
                {
                    { "positive", new List<string> { "😄", "😎", "🔥", "👏", "💪", "⭐" } },
                    { "neutral", new List<string> { "👀", "👍", "🎮" } },
                    { "gaming", new List<string> { "🎮", "🎯", "🏆", "🎪" } }
                },
                Punctuation = new List<string> { "!", "!!", "..." },
                CommonWords = new Dictionary<string, List<string>>
                {
                    { "greetings", new List<string> { "hey", "hi", "hello", "sup" } },
                    { "reactions", new List<string> { "nice", "cool", "wow", "gg" } }
                }
            };
        }

        private ProxiesFile GetDefaultProxies()
        {
            return new ProxiesFile
            {
                Proxies = new List<ProxyEntry>(),
                Settings = new ProxySettingsFile 
                { 
                    ProxyEnabled = false, 
                    UseRandomProxy = true, 
                    RotateOnError = true, 
                    ProxyTimeout = 10, 
                    MaxErrorCount = 3 
                }
            };
        }

        private IWebProxy BuildProxyForAccount(Account account)
        {
            if (!UseProxies) 
            {
                System.Diagnostics.Debug.WriteLine("BuildProxyForAccount: Proxies desactivados por usuario");
                return null;
            }
            if (Proxies == null || Proxies.Settings == null || !Proxies.Settings.ProxyEnabled) 
            {
                System.Diagnostics.Debug.WriteLine("BuildProxyForAccount: Proxies no configurados o deshabilitados");
                return null;
            }
            var active = Proxies.Proxies?.Where(p => p.Active && p.ErrorCount < (Proxies.Settings.MaxErrorCount > 0 ? Proxies.Settings.MaxErrorCount : 3)).ToList();
            if (active == null || active.Count == 0) 
            {
                System.Diagnostics.Debug.WriteLine("BuildProxyForAccount: No hay proxies activos disponibles");
                return null;
            }
            var pick = Proxies.Settings.UseRandomProxy
                ? active[random.Next(active.Count)]
                : active.OrderBy(p => p.LastUsed ?? 0).First();
            pick.LastUsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var uri = new Uri($"http://{pick.Host}:{pick.Port}");
            var proxy = new WebProxy(uri);
            if (!string.IsNullOrEmpty(pick.Username))
            {
                proxy.Credentials = new NetworkCredential(pick.Username, pick.Password);
            }
            System.Diagnostics.Debug.WriteLine($"BuildProxyForAccount: Usando proxy {pick.Host}:{pick.Port}");
            return proxy;
        }

        private ProxyEntry SelectProxyForAccount(Account account)
        {
            if (!UseProxies) return null;
            if (Proxies == null || Proxies.Settings == null || !Proxies.Settings.ProxyEnabled) return null;
            var active = Proxies.Proxies?.Where(p => p.Active && p.ErrorCount < (Proxies.Settings.MaxErrorCount > 0 ? Proxies.Settings.MaxErrorCount : 3)).ToList();
            if (active == null || active.Count == 0) return null;
            var pick = Proxies.Settings.UseRandomProxy
                ? active[random.Next(active.Count)]
                : active.OrderBy(p => p.LastUsed ?? 0).First();
            pick.LastUsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return pick;
        }

        private string GetKickEmoteOnly()
        {
            var all = new List<string>();
            if (Messages?.KickEmotes != null)
            {
                foreach (var kv in Messages.KickEmotes)
                {
                    all.AddRange(kv.Value);
                }
            }
            if (all.Count == 0)
            {
                all.AddRange(new[] { "[emote:37248:ratJAM]", "[emote:39268:HYPERCLAPH]", "[emote:39284:vibePlz]", "[emote:1730752:emojiAngel]", "[emote:39273:MuteD]" });
            }
            return all[random.Next(all.Count)];
        }

        private string GetPersonalityMessage(string personality)
        {
            if (Config.Behavior.OnlyEmotes) return GetKickEmoteOnly();

            // 15% solo emote
            if (random.NextDouble() < 0.15)
            {
                return GetKickEmoteOnly();
            }

            var personalities = Messages?.Personalities ?? new Dictionary<string, List<string>>();
            List<string> list;
            if (!personalities.TryGetValue(personality ?? "", out list))
            {
                personalities.TryGetValue("casual", out list);
            }
            if (list == null || list.Count == 0) list = new List<string> { "nice! 👍" };
            var baseMsg = list[random.Next(list.Count)];

            if (!Config.Behavior.AddHumanVariations || Config.Behavior.OnlyEmotes)
            {
                return baseMsg;
            }

            // Variaciones humanas
            var message = baseMsg;
            if (random.NextDouble() < 0.15)
            {
                message = random.Next(2) == 0 ? message.ToUpperInvariant() : message.ToLowerInvariant();
            }
            var allEmojis = new List<string>();
            if (Messages?.Emojis != null)
            {
                foreach (var kv in Messages.Emojis) allEmojis.AddRange(kv.Value);
            }
            var allKickEmotes = new List<string>();
            if (Messages?.KickEmotes != null)
            {
                foreach (var kv in Messages.KickEmotes) allKickEmotes.AddRange(kv.Value);
            }
            if (random.NextDouble() < 0.50)
            {
                if (random.NextDouble() < 0.30 && allEmojis.Count > 0)
                {
                    message += " " + allEmojis[random.Next(allEmojis.Count)];
                }
                else if (allKickEmotes.Count > 0)
                {
                    message += " " + allKickEmotes[random.Next(allKickEmotes.Count)];
                }
            }
            if (random.NextDouble() < 0.10 && Messages?.Punctuation != null && Messages.Punctuation.Count > 0)
            {
                message += Messages.Punctuation[random.Next(Messages.Punctuation.Count)];
            }
            return message;
        }

        public async Task<string> StartChatbotOnceBatchAsync(string channel, Action<string> log, CancellationToken ct)
        {
            if (Accounts == null || Accounts.Count == 0) return "No hay cuentas en kicks.json";

            var first = Accounts[0];
            IApiClient api = new KickApiClient(BuildProxyForAccount(first));
            try
            {
                // Intento 1: HttpClient normal
                var info = await api.GetChannelInfoAsync(channel, first.Token, ct).ConfigureAwait(false);
                if (info.Item2 != null && info.Item2.Contains("Request blocked by security policy"))
                {
                    log("⚠️ Bloqueo Cloudflare detectado en HttpClient. Cambiando a cURL...");
                    api.Dispose();
                    api = new CurlApiClient();
                    // Reintentar con cURL
                    info = await api.GetChannelInfoAsync(channel, first.Token, ct).ConfigureAwait(false);
                }
                if (info.Item2 != null) return info.Item2;
                var chatroomId = info.Item1?.ChatRoom?.Id ?? 0;
                if (chatroomId <= 0) return "No se pudo obtener chatroom ID";
                
                log($"✅ Canal: {channel} (ID: {info.Item1.Id})");
                log($"💬 Chatroom ID: {chatroomId}");
                log($"🔴 En vivo: {(info.Item1.LiveStream != null ? "Sí" : "No")}");

                var accountsOrdered = Accounts.ToList();
                if (Config.Behavior.RandomizeOrder) accountsOrdered = accountsOrdered.OrderBy(_ => random.Next()).ToList();

                var concurrent = Math.Max(1, Config.Behavior.MaxConcurrentBots);
                for (int i = 0; i < accountsOrdered.Count; i += concurrent)
                {
                    var batch = accountsOrdered.Skip(i).Take(concurrent).ToList();
                    var tasks = new List<Task>();
                    foreach (var acc in batch)
                    {
                        tasks.Add(RunBotAsync(api, acc, chatroomId, log, ct));
                    }
                    await Task.WhenAll(tasks).ConfigureAwait(false);

                    if (i + concurrent < accountsOrdered.Count)
                    {
                        var pause = RandomBetween(Config.Delays.BatchPause.Min, Config.Delays.BatchPause.Max);
                        log($"⏸️ Pausa entre tandas: {pause/60} minutos");
                        await Task.Delay(TimeSpan.FromSeconds(pause), ct).ConfigureAwait(false);
                    }
                }
                return null;
            }
            finally
            {
                api?.Dispose();
            }
        }

        private async Task RunBotAsync(IApiClient sharedApi, Account account, long chatroomId, Action<string> log, CancellationToken ct)
        {
            try
            {
                var initial = RandomBetween(Config.Delays.InitialWait.Min, Config.Delays.InitialWait.Max);
                log($"⏳ {account.Username} esperando {initial}s...");
                await Task.Delay(TimeSpan.FromSeconds(initial), ct).ConfigureAwait(false);

                var maxMessages = Math.Max(1, Config.Behavior.MessagesPerSession);
                var sent = 0; var retries = 0; var maxRetries = Math.Max(1, Config.Safety.MaxRetriesPerAccount);
                while (sent < maxMessages && retries < maxRetries && !ct.IsCancellationRequested)
                {
                    var msg = GetPersonalityMessage(account.Personality);
                    var typing = RandomBetweenDouble(Config.Delays.TypingSimulation.Min, Config.Delays.TypingSimulation.Max);
                    log($"⌨️ {account.Username} escribiendo '{msg}'... ({typing:F1}s)");
                    await Task.Delay(TimeSpan.FromSeconds(typing), ct).ConfigureAwait(false);

                    // Construir cliente cURL con proxy si estamos en modo cURL y hay proxy
                    var proxyEntry = SelectProxyForAccount(account);
                    var usingCurl = sharedApi is CurlApiClient;
                    IApiClient clientForSend = sharedApi;
                    if (usingCurl && proxyEntry != null)
                    {
                        clientForSend = new CurlApiClient(proxyEntry.Host, proxyEntry.Port, proxyEntry.Username, proxyEntry.Password);
                    }

                    var result = await clientForSend.SendMessageAsync(chatroomId, msg, account.Token, ct).ConfigureAwait(false);
                    
                    if (result.Item1)
                    {
                        sent += 1; retries = 0;
                        // Log IP/proxy info
                        if (proxyEntry != null)
                        {
                            log($"✅ [{account.Username}]: {msg} via {proxyEntry.Host}:{proxyEntry.Port}");
                        }
                        else
                        {
                            log($"✅ [{account.Username}]: {msg} (sin proxy)");
                        }
                        account.MessagesSent = (account.MessagesSent) + 1;
                        account.LastMessageTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    }
                    else
                    {
                        retries += 1;
                        var status = result.Item2;
                        var response = result.Item3;
                        log($"❌ {account.Username} FALLÓ (intento {retries}/{maxRetries}) - Status: {status}");
                        log($"   📋 Respuesta: {response}");
                        if (status == 401)
                        {
                            log("   🔑 Causa: Token inválido o expirado");
                            break;
                        }
                        if (status == 403)
                        {
                            log("   🚫 Causa: Sin permisos en el canal");
                            log("   💡 Posibles causas:");
                            log("      - Canal en modo followers-only o subscribers-only");
                            log("      - Cuenta bloqueada/baneada");
                            log("      - Token expirado o inválido");
                            log("      - Problemas con proxy (si está activado)");
                            log("   🔧 Soluciones:");
                            log("      1. Prueba enviar mensaje manualmente con esa cuenta en kick.com");
                            log("      2. Desactiva proxies temporalmente");
                            log("      3. Verifica que el token sea válido");
                            break;
                        }
                        if (status == 404)
                        {
                            log("   🔍 Causa: Canal/chatroom no encontrado");
                            break;
                        }
                        if (status == 429 && Config.Safety.AutoRetryOn429)
                        {
                            log("   ⏰ Rate limiting - esperando 120s...");
                            await Task.Delay(TimeSpan.FromSeconds(120), ct).ConfigureAwait(false);
                        }
                        if (retries >= maxRetries) break;
                    }

                    if (sent < maxMessages)
                    {
                        var wait = RandomBetween(Config.Delays.BetweenMessages.Min, Config.Delays.BetweenMessages.Max);
                        log($"💤 {account.Username} descansando {wait/60}m {wait%60}s...");
                        await Task.Delay(TimeSpan.FromSeconds(wait), ct).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                log($"❌ Error en bot {account.Username}: {ex.Message}");
            }
        }

        public async Task<string> StartRotatingAsync(string channel, Action<string> log, CancellationToken ct)
        {
            if (Accounts == null || Accounts.Count == 0) return "No hay cuentas en kicks.json";
            var first = Accounts[0];
            IApiClient api = new KickApiClient(BuildProxyForAccount(first));
            try
            {
                var info = await api.GetChannelInfoAsync(channel, first.Token, ct).ConfigureAwait(false);
                if (info.Item2 != null && (info.Item2.Contains("Request blocked") || info.Item2.Contains("403")))
                {
                    log("⚠️ Bloqueo detectado en HttpClient. Cambiando a cURL para modo rotativo...");
                    api.Dispose();
                    api = new CurlApiClient();
                    info = await api.GetChannelInfoAsync(channel, first.Token, ct).ConfigureAwait(false);
                }
                if (info.Item2 != null) return info.Item2;
                var chatroomId = info.Item1?.ChatRoom?.Id ?? 0;
                if (chatroomId <= 0) return "No se pudo obtener chatroom ID";

                var accounts = Accounts.ToList();
                if (Config.Behavior.RandomizeOrder) accounts = accounts.OrderBy(_ => random.Next()).ToList();

                var idx = 0;
                while (!ct.IsCancellationRequested)
                {
                    var acc = accounts[idx];
                    var msg = GetPersonalityMessage(acc.Personality);
                    var typing = RandomBetweenDouble(Config.Delays.TypingSimulation.Min, Config.Delays.TypingSimulation.Max);
                    log($"⌨️ {acc.Username} escribiendo '{msg}'... ({typing:F1}s)");
                    await Task.Delay(TimeSpan.FromSeconds(typing), ct).ConfigureAwait(false);

                    // Seleccionar proxy si cURL está activo
                    var proxyEntry = SelectProxyForAccount(acc);
                    IApiClient clientForSend = api;
                    if (api is CurlApiClient && proxyEntry != null)
                    {
                        clientForSend = new CurlApiClient(proxyEntry.Host, proxyEntry.Port, proxyEntry.Username, proxyEntry.Password);
                    }
                    var result = await clientForSend.SendMessageAsync(chatroomId, msg, acc.Token, ct).ConfigureAwait(false);
                    if (result.Item1)
                    {
                        if (proxyEntry != null)
                        {
                            log($"✅ [{acc.Username}]: {msg} via {proxyEntry.Host}:{proxyEntry.Port}");
                        }
                        else
                        {
                            log($"✅ [{acc.Username}]: {msg} (sin proxy)");
                        }
                        acc.MessagesSent = (acc.MessagesSent) + 1;
                        acc.LastMessageTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    }
                    else
                    {
                        log($"❌ {acc.Username} FALLÓ - Status: {result.Item2}");
                        if (result.Item2 == 401)
                        {
                            log("   🔑 Causa: Token inválido o expirado");
                        }
                        else if (result.Item2 == 403)
                        {
                            log("   🚫 Causa: Sin permisos en el canal (followers-only, sub-only, mute/ban o restricciones)");
                            log("   💡 Prueba sin proxy y valida que la cuenta pueda hablar en el canal");
                        }
                        else if (result.Item2 == 404)
                        {
                            log("   🔍 Causa: Canal/chatroom no encontrado");
                            break;
                        }
                        if (result.Item2 == 429 && Config.Safety.AutoRetryOn429)
                        {
                            log("   ⏰ Rate limiting - esperando 120s...");
                            await Task.Delay(TimeSpan.FromSeconds(120), ct).ConfigureAwait(false);
                        }
                    }

                    idx = (idx + 1) % accounts.Count;
                    var wait = new Random().Next(5, 11);
                    var next = accounts[idx].Username;
                    log($"⏳ Esperando {wait}s... Próximo: {next}");
                    await Task.Delay(TimeSpan.FromSeconds(wait), ct).ConfigureAwait(false);
                }
                return null;
            }
            finally
            {
                api?.Dispose();
            }
        }

        public bool SaveAccounts()
        {
            return JsonStorage.WriteJsonFile(Path.Combine(basePath, "kicks.json"), Accounts);
        }

        public void UpdateConfig(AppConfig newConfig)
        {
            if (newConfig == null) return;
            Config = newConfig;
        }

        private int RandomBetween(int min, int max) => (min <= max) ? new Random().Next(min, max + 1) : min;
        private double RandomBetweenDouble(int min, int max)
        {
            if (min > max) return min;
            var r = new Random();
            return min + r.NextDouble() * Math.Max(0, (max - min));
        }
    }

    // Removed noisy debug trace listener from UI
}

