using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kick_ChatBOT.Services;
using Kick_ChatBOT.Models;
using System.IO;

namespace Kick_ChatBOT
{
    public partial class ChatBOT : MetroFramework.Forms.MetroForm
    {
        private ChatbotEngine engine;
        private CancellationTokenSource cts;

        public ChatBOT()
        {
            InitializeComponent();
        }

        private void ChatBOT_Load(object sender, EventArgs e)
        {

            engine = new ChatbotEngine(AppDomain.CurrentDomain.BaseDirectory + "KICK CHATBOT\\");

            // Restaurar rutas personalizadas del usuario
            var saved = UserPreferences.LoadPaths();
            
            if (saved != null)
            {
                if (!string.IsNullOrEmpty(saved.Kicks)) 
                {
                    engine.KicksPath = saved.Kicks;
                }
                if (!string.IsNullOrEmpty(saved.Proxies)) 
                {
                    engine.ProxiesPath = saved.Proxies;
                }
                if (!string.IsNullOrEmpty(saved.Messages))
                {
                    engine.MessagesPath = saved.Messages;
                }
                if (!string.IsNullOrEmpty(saved.Config))
                {
                    engine.ConfigPath = saved.Config;
                }
                if (!string.IsNullOrEmpty(saved.CategoryMessages))
                {
                    engine.CategoryMessagesPath = saved.CategoryMessages;
                }
                if (!string.IsNullOrEmpty(saved.Greetings))
                {
                    engine.GreetingsPath = saved.Greetings;
                }
            }
            engine.LoadAll();
            LogCounts();

            btnStart.Click += async (s, ev) => await StartNormalAsync();
            btnStartRotating.Click += async (s, ev) => await StartRotatingAsync();
            btnStop.Click += (s, ev) => StopTasks();

            btnLoadKicks.Click += (s, ev) => SelectAndLoadJson("kicks.json", path => engine.KicksPath = path, () => Log("kicks.json cargado"));
            btnLoadProxies.Click += (s, ev) => SelectAndLoadJson("proxies.json", path => engine.ProxiesPath = path, () => { engine.UseProxies = true; Log("proxies.json cargado"); });
            btnLoadMessages.Click += (s, ev) => SelectAndLoadJson("messages.json", path => engine.MessagesPath = path, () => Log("messages.json cargado"));
            btnLoadConfig.Click += (s, ev) => OpenConfigEditor();
            btnLoadCategoryMessages.Click += (s, ev) => SelectAndLoadJson("category_messages.json", path => engine.CategoryMessagesPath = path, () => Log("category_messages.json cargado"));
            btnLoadGreetings.Click += (s, ev) => SelectAndLoadJson("greetings.json", path => engine.GreetingsPath = path, () => Log("greetings.json cargado"));
            btnSendManual.Click += async (s, ev) => await SendManualAsync();

            // toggle proxies
            toggleProxy.CheckedChanged += (s, ev) =>
            {
                engine.UseProxies = toggleProxy.Checked;
                Log(engine.UseProxies ? "Proxies ACTIVADOS" : "Proxies DESACTIVADOS");
            };

            // Botones de diagnóstico removidos para una UI más limpia

        }

        private async Task StartNormalAsync()
        {
            StopTasks();
            cts = new CancellationTokenSource();
            var channel = txtChannel.Text?.Trim();
            if (string.IsNullOrEmpty(channel)) { Log("Canal requerido"); return; }
            Log($"Iniciando chatbot en {channel}...");
            var err = await engine.StartChatbotOnceBatchAsync(channel, Log, cts.Token);
            if (!string.IsNullOrEmpty(err)) Log("Error: " + err);
            engine.SaveAccounts();
        }

        private async Task StartRotatingAsync()
        {
            StopTasks();
            cts = new CancellationTokenSource();
            var channel = txtChannel.Text?.Trim();
            if (string.IsNullOrEmpty(channel)) { Log("Canal requerido"); return; }
            Log($"Iniciando rotativo en {channel}...");
            _ = Task.Run(async () =>
            {
                var err = await engine.StartRotatingAsync(channel, Log, cts.Token);
                if (!string.IsNullOrEmpty(err)) Log("Error: " + err);
                engine.SaveAccounts();
            });
        }

        private async Task SendManualAsync()
        {
            var channel = txtChannel.Text?.Trim();
            var msg = txtManualMessage.Text?.Trim();
            if (string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(msg)) { Log("Canal y mensaje requeridos"); return; }
            var err = await engine.SendManualMessageAsync(channel, msg, Log, CancellationToken.None);
            if (!string.IsNullOrEmpty(err)) Log("Error: " + err);
        }

        private void StopTasks()
        {
            try { cts?.Cancel(); } catch { }
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(Log), message);
                return;
            }
            if (lstLog.Items.Count > 2000) lstLog.Items.Clear();
            lstLog.Items.Add(message);
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        private void SelectAndLoadJson(string defaultName, Action<string> setPath, Action onLoaded)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "JSON (*.json)|*.json|Todos los archivos (*.*)|*.*";
                dlg.Title = "Selecciona " + defaultName;
                dlg.FileName = defaultName;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    setPath(dlg.FileName);
                    engine.LoadAll();
                    onLoaded?.Invoke();

                    // Guardar rutas personalizadas
                    var current = UserPreferences.LoadPaths() ?? new UserPaths();
                    if (defaultName == "kicks.json") current.Kicks = dlg.FileName;
                    else if (defaultName == "proxies.json") current.Proxies = dlg.FileName;
                    else if (defaultName == "messages.json") current.Messages = dlg.FileName;
                    else if (defaultName == "config.json") current.Config = dlg.FileName;
                    else if (defaultName == "category_messages.json") current.CategoryMessages = dlg.FileName;
                    else if (defaultName == "greetings.json") current.Greetings = dlg.FileName;
                    UserPreferences.SavePaths(current);

                    LogCounts();
                }
            }
        }

        private void LogCounts()
        {
            var accountsCount = engine.Accounts?.Count ?? 0;
            var proxiesTotal = engine.Proxies?.Proxies?.Count ?? 0;
            var proxiesActive = 0;
            if (engine.Proxies?.Proxies != null)
            {
                foreach (var p in engine.Proxies.Proxies) if (p.Active) proxiesActive++;
            }
            var personalities = engine.Messages?.Personalities?.Count ?? 0;
            var emoteCats = engine.Messages?.KickEmotes?.Count ?? 0;
            var categoryCount = engine.CategoryMessages?.Count ?? 0;
            var greetingsCount = engine.Greetings?.Greetings?.Count ?? 0;
            var farewellsCount = engine.Greetings?.Farewells?.Count ?? 0;
            
            Log($"=== ESTADO ACTUAL ===");
            
            if (accountsCount > 0)
            {
                Log($"✅ Cuentas: {accountsCount} (desde TU archivo)");
            }
            else
            {
                Log($"⚠️ SIN CUENTAS - Selecciona tu kicks.json");
            }
            
            Log($"📡 Proxies: {proxiesActive}/{proxiesTotal} activos");
            Log($"💬 Personalidades: {personalities}");
            Log($"🎭 Emotes: {emoteCats} categorías");
            Log($"📚 Categorías: {categoryCount}");
            Log($"👋 Saludos: {greetingsCount} | Despedidas: {farewellsCount}");
            
            if (accountsCount == 0)
            {
                Log($"👆 IMPORTANTE: Usa 'Cargar kicks.json' para seleccionar TU archivo");
            }
            
            Log($"====================");
        }

        private string GetFileSource(string fileName, string customPath)
        {
            if (!string.IsNullOrEmpty(customPath) && File.Exists(customPath))
            {
                return $"PERSONALIZADA ({Path.GetFileName(customPath)})";
            }
            return $"DEFAULT ({fileName})";
        }

        private void OpenConfigEditor()
        {
            var path = engine.ConfigPath;
            var cfg = engine.Config ?? new AppConfig
            {
                Delays = new Delays
                {
                    InitialWait = new DelayRange { Min = 1, Max = 2 },
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

            using (var dlg = new Kick_ChatBOT.Forms.ConfigForm(cfg, path))
            {
                dlg.ShowDialog(this);
                if (dlg.Saved && dlg.ResultConfig != null)
                {
                    engine.UpdateConfig(dlg.ResultConfig);
                    if (!string.IsNullOrEmpty(dlg.ResultPath))
                    {
                        engine.ConfigPath = dlg.ResultPath;
                        var current = UserPreferences.LoadPaths() ?? new UserPaths();
                        current.Config = dlg.ResultPath;
                        UserPreferences.SavePaths(current);
                    }
                    Log("✅ Configuración actualizada");
                }
            }
        }

        private async Task TestConnection()
        {
            Log("🔧 INICIANDO TEST DE CONECTIVIDAD...");
            try
            {
                using (var api = new Services.KickApiClient())
                {
                    var result = await api.TestConnectivityAsync(new CancellationTokenSource().Token);
                    Log(result.Item1 ? $"✅ {result.Item2}" : $"❌ {result.Item2}");
                }
            }
            catch (Exception ex)
            {
                Log($"💥 Error en test: {ex.Message}");
            }
            Log("🔧 TEST COMPLETADO");
        }


    }
}


