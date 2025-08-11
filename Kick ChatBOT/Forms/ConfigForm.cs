using System;
using System.Drawing;
using System.Windows.Forms;
using Kick_ChatBOT.Models;
using Kick_ChatBOT.Services;

namespace Kick_ChatBOT.Forms
{
    public class ConfigForm : Form
    {
        private AppConfig working;
        private string currentPath;

        public AppConfig ResultConfig { get; private set; }
        public string ResultPath { get; private set; }
        public bool Saved { get; private set; }

        // Delays controls
        private NumericUpDown numInitMin, numInitMax, numBetweenMin, numBetweenMax, numTypingMin, numTypingMax, numBetweenAccounts, numBatchMin, numBatchMax;
        // Behavior
        private NumericUpDown numConcurrent, numMsgsPerSession;
        private CheckBox chkVariations, chkRandomize, chkOnlyEmotes;
        // Safety
        private CheckBox chkRespectLimits, chkAutoRetry429, chkStopOnTokenError;
        private NumericUpDown numMaxRetriesPerAccount;
        // Proxy
        private CheckBox chkProxyEnabled, chkProxyAutoAssign, chkProxyShowInfo, chkProxyFallback;
        private NumericUpDown numProxyMaxRetries;

        private Label lblPath;

        public ConfigForm(AppConfig config, string path)
        {
            Text = "Configurar chatbot";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Size = new Size(580, 520);

            working = DeepCopy(config);
            currentPath = path;
            BuildUi();
            LoadToControls(working);
        }

        private void BuildUi()
        {
            var tabs = new TabControl { Dock = DockStyle.Top, Height = 420 };
            var tabDelays = new TabPage("Delays");
            var tabBehavior = new TabPage("Behavior");
            var tabSafety = new TabPage("Safety");
            var tabProxy = new TabPage("Proxy");
            tabs.TabPages.AddRange(new[] { tabDelays, tabBehavior, tabSafety, tabProxy });

            // Helper creators
            NumericUpDown CreateNum(int min, int max, int step) => new NumericUpDown { Minimum = min, Maximum = max, Increment = step, Width = 80 };
            CheckBox CreateChk(string text) => new CheckBox { Text = text, AutoSize = true };

            // Delays layout
            var pnlD = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 6, AutoSize = true };
            pnlD.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            pnlD.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            pnlD.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            pnlD.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            int r = 0;
            pnlD.Controls.Add(new Label { Text = "Inicial (s):", AutoSize = true }, 0, r);
            numInitMin = CreateNum(0, 3600, 1); numInitMax = CreateNum(0, 3600, 1);
            pnlD.Controls.Add(new Label { Text = "min", AutoSize = true }, 1, r);
            pnlD.Controls.Add(numInitMin, 2, r);
            pnlD.Controls.Add(new Label { Text = "max", AutoSize = true }, 1, ++r);
            pnlD.Controls.Add(numInitMax, 2, r);

            r++;
            pnlD.Controls.Add(new Label { Text = "Entre mensajes (s):", AutoSize = true }, 0, r);
            numBetweenMin = CreateNum(0, 3600, 1); numBetweenMax = CreateNum(0, 3600, 1);
            pnlD.Controls.Add(new Label { Text = "min", AutoSize = true }, 1, r);
            pnlD.Controls.Add(numBetweenMin, 2, r);
            pnlD.Controls.Add(new Label { Text = "max", AutoSize = true }, 1, ++r);
            pnlD.Controls.Add(numBetweenMax, 2, r);

            r++;
            pnlD.Controls.Add(new Label { Text = "Simulación teclado (s):", AutoSize = true }, 0, r);
            numTypingMin = CreateNum(0, 60, 1); numTypingMax = CreateNum(0, 60, 1);
            pnlD.Controls.Add(new Label { Text = "min", AutoSize = true }, 1, r);
            pnlD.Controls.Add(numTypingMin, 2, r);
            pnlD.Controls.Add(new Label { Text = "max", AutoSize = true }, 1, ++r);
            pnlD.Controls.Add(numTypingMax, 2, r);

            r++;
            pnlD.Controls.Add(new Label { Text = "Entre cuentas (s):", AutoSize = true }, 0, r);
            numBetweenAccounts = CreateNum(0, 3600, 1);
            pnlD.Controls.Add(numBetweenAccounts, 2, r);

            r++;
            pnlD.Controls.Add(new Label { Text = "Pausa entre tandas (s):", AutoSize = true }, 0, r);
            numBatchMin = CreateNum(0, 7200, 1); numBatchMax = CreateNum(0, 7200, 1);
            pnlD.Controls.Add(new Label { Text = "min", AutoSize = true }, 1, r);
            pnlD.Controls.Add(numBatchMin, 2, r);
            pnlD.Controls.Add(new Label { Text = "max", AutoSize = true }, 1, ++r);
            pnlD.Controls.Add(numBatchMax, 2, r);
            tabDelays.Controls.Add(pnlD);

            // Behavior
            var pnlB = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true };
            pnlB.Controls.Add(new Label { Text = "Bots simultáneos:" });
            numConcurrent = CreateNum(1, 1000, 1); pnlB.Controls.Add(numConcurrent);
            pnlB.Controls.Add(new Label { Text = "Mensajes por sesión:" });
            numMsgsPerSession = CreateNum(1, 1000000, 1); pnlB.Controls.Add(numMsgsPerSession);
            chkVariations = CreateChk("Variaciones humanas"); pnlB.Controls.Add(chkVariations);
            chkRandomize = CreateChk("Orden aleatorio"); pnlB.Controls.Add(chkRandomize);
            chkOnlyEmotes = CreateChk("Solo emotes de Kick"); pnlB.Controls.Add(chkOnlyEmotes);
            tabBehavior.Controls.Add(pnlB);

            // Safety
            var pnlS = new FlowLayoutPanel { Dock = DockStyle.Fill };
            chkRespectLimits = CreateChk("Respetar rate limits"); pnlS.Controls.Add(chkRespectLimits);
            chkAutoRetry429 = CreateChk("Auto-retry 429"); pnlS.Controls.Add(chkAutoRetry429);
            pnlS.Controls.Add(new Label { Text = "Max reintentos por cuenta:" });
            numMaxRetriesPerAccount = CreateNum(0, 100, 1); pnlS.Controls.Add(numMaxRetriesPerAccount);
            chkStopOnTokenError = CreateChk("Detener en error de token"); pnlS.Controls.Add(chkStopOnTokenError);
            tabSafety.Controls.Add(pnlS);

            // Proxy
            var pnlP = new FlowLayoutPanel { Dock = DockStyle.Fill };
            chkProxyEnabled = CreateChk("Proxies habilitados"); pnlP.Controls.Add(chkProxyEnabled);
            chkProxyAutoAssign = CreateChk("Auto asignar proxy"); pnlP.Controls.Add(chkProxyAutoAssign);
            chkProxyShowInfo = CreateChk("Mostrar info de proxy"); pnlP.Controls.Add(chkProxyShowInfo);
            chkProxyFallback = CreateChk("Fallback a directo"); pnlP.Controls.Add(chkProxyFallback);
            pnlP.Controls.Add(new Label { Text = "Max reintentos por proxy:" });
            numProxyMaxRetries = CreateNum(0, 100, 1); pnlP.Controls.Add(numProxyMaxRetries);
            tabProxy.Controls.Add(pnlP);

            // Bottom bar
            lblPath = new Label { AutoSize = true, Text = "Archivo: " + (string.IsNullOrEmpty(currentPath) ? "(no guardado)" : currentPath) };
            var btnLoad = new Button { Text = "Abrir...", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            var btnSave = new Button { Text = "Guardar", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            var btnSaveAs = new Button { Text = "Guardar como...", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            var btnClose = new Button { Text = "Cerrar", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };

            btnLoad.Click += (s, e) => LoadFromFile();
            btnSave.Click += (s, e) => SaveToFile(currentPath, promptWhenEmpty: true);
            btnSaveAs.Click += (s, e) => SaveToFile(null, promptWhenEmpty: true);
            btnClose.Click += (s, e) => Close();

            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6) };
            bottom.Controls.Add(btnClose);
            bottom.Controls.Add(btnSaveAs);
            bottom.Controls.Add(btnSave);
            bottom.Controls.Add(btnLoad);
            var header = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 24 };
            header.Controls.Add(lblPath);

            Controls.Add(tabs);
            Controls.Add(header);
            Controls.Add(bottom);
        }

        private void LoadToControls(AppConfig cfg)
        {
            numInitMin.Value = cfg.Delays.InitialWait.Min; numInitMax.Value = cfg.Delays.InitialWait.Max;
            numBetweenMin.Value = cfg.Delays.BetweenMessages.Min; numBetweenMax.Value = cfg.Delays.BetweenMessages.Max;
            numTypingMin.Value = cfg.Delays.TypingSimulation.Min; numTypingMax.Value = cfg.Delays.TypingSimulation.Max;
            numBetweenAccounts.Value = cfg.Delays.BetweenAccounts;
            numBatchMin.Value = cfg.Delays.BatchPause.Min; numBatchMax.Value = cfg.Delays.BatchPause.Max;

            numConcurrent.Value = cfg.Behavior.MaxConcurrentBots;
            numMsgsPerSession.Value = cfg.Behavior.MessagesPerSession;
            chkVariations.Checked = cfg.Behavior.AddHumanVariations;
            chkRandomize.Checked = cfg.Behavior.RandomizeOrder;
            chkOnlyEmotes.Checked = cfg.Behavior.OnlyEmotes;

            chkRespectLimits.Checked = cfg.Safety.RespectRateLimits;
            chkAutoRetry429.Checked = cfg.Safety.AutoRetryOn429;
            numMaxRetriesPerAccount.Value = cfg.Safety.MaxRetriesPerAccount;
            chkStopOnTokenError.Checked = cfg.Safety.StopOnTokenError;

            chkProxyEnabled.Checked = cfg.Proxy.Enabled;
            chkProxyAutoAssign.Checked = cfg.Proxy.AutoAssign;
            chkProxyShowInfo.Checked = cfg.Proxy.ShowProxyInfo;
            chkProxyFallback.Checked = cfg.Proxy.FallbackToDirect;
            numProxyMaxRetries.Value = cfg.Proxy.MaxRetriesPerProxy;
        }

        private AppConfig ReadFromControls()
        {
            working.Delays.InitialWait.Min = (int)numInitMin.Value; working.Delays.InitialWait.Max = (int)numInitMax.Value;
            working.Delays.BetweenMessages.Min = (int)numBetweenMin.Value; working.Delays.BetweenMessages.Max = (int)numBetweenMax.Value;
            working.Delays.TypingSimulation.Min = (int)numTypingMin.Value; working.Delays.TypingSimulation.Max = (int)numTypingMax.Value;
            working.Delays.BetweenAccounts = (int)numBetweenAccounts.Value;
            working.Delays.BatchPause.Min = (int)numBatchMin.Value; working.Delays.BatchPause.Max = (int)numBatchMax.Value;

            working.Behavior.MaxConcurrentBots = (int)numConcurrent.Value;
            working.Behavior.MessagesPerSession = (int)numMsgsPerSession.Value;
            working.Behavior.AddHumanVariations = chkVariations.Checked;
            working.Behavior.RandomizeOrder = chkRandomize.Checked;
            working.Behavior.OnlyEmotes = chkOnlyEmotes.Checked;

            working.Safety.RespectRateLimits = chkRespectLimits.Checked;
            working.Safety.AutoRetryOn429 = chkAutoRetry429.Checked;
            working.Safety.MaxRetriesPerAccount = (int)numMaxRetriesPerAccount.Value;
            working.Safety.StopOnTokenError = chkStopOnTokenError.Checked;

            working.Proxy.Enabled = chkProxyEnabled.Checked;
            working.Proxy.AutoAssign = chkProxyAutoAssign.Checked;
            working.Proxy.ShowProxyInfo = chkProxyShowInfo.Checked;
            working.Proxy.FallbackToDirect = chkProxyFallback.Checked;
            working.Proxy.MaxRetriesPerProxy = (int)numProxyMaxRetries.Value;

            return working;
        }

        private void LoadFromFile()
        {
            using (var dlg = new OpenFileDialog { Filter = "JSON (*.json)|*.json|Todos (*.*)|*.*", Title = "Abrir config.json" })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var cfg = JsonStorage.ReadJsonFile<AppConfig>(dlg.FileName, working);
                    working = DeepCopy(cfg);
                    currentPath = dlg.FileName;
                    lblPath.Text = "Archivo: " + currentPath;
                    LoadToControls(working);
                }
            }
        }

        private void SaveToFile(string path, bool promptWhenEmpty)
        {
            var cfg = ReadFromControls();
            if (string.IsNullOrEmpty(path) && promptWhenEmpty)
            {
                using (var dlg = new SaveFileDialog { Filter = "JSON (*.json)|*.json", FileName = "config.json", Title = "Guardar config.json" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    path = dlg.FileName;
                }
            }

            if (!string.IsNullOrEmpty(path))
            {
                if (JsonStorage.WriteJsonFile(path, cfg))
                {
                    currentPath = path;
                    lblPath.Text = "Archivo: " + currentPath;
                    Saved = true;
                    ResultConfig = DeepCopy(cfg);
                    ResultPath = currentPath;
                    MessageBox.Show(this, "Configuración guardada", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "No se pudo guardar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static AppConfig DeepCopy(AppConfig c)
        {
            return new AppConfig
            {
                Delays = new Delays
                {
                    InitialWait = new DelayRange { Min = c.Delays.InitialWait.Min, Max = c.Delays.InitialWait.Max },
                    BetweenMessages = new DelayRange { Min = c.Delays.BetweenMessages.Min, Max = c.Delays.BetweenMessages.Max },
                    TypingSimulation = new DelayRange { Min = c.Delays.TypingSimulation.Min, Max = c.Delays.TypingSimulation.Max },
                    BetweenAccounts = c.Delays.BetweenAccounts,
                    BatchPause = new DelayRange { Min = c.Delays.BatchPause.Min, Max = c.Delays.BatchPause.Max }
                },
                Behavior = new Behavior
                {
                    MaxConcurrentBots = c.Behavior.MaxConcurrentBots,
                    MessagesPerSession = c.Behavior.MessagesPerSession,
                    AddHumanVariations = c.Behavior.AddHumanVariations,
                    RandomizeOrder = c.Behavior.RandomizeOrder,
                    OnlyEmotes = c.Behavior.OnlyEmotes
                },
                Safety = new Safety
                {
                    RespectRateLimits = c.Safety.RespectRateLimits,
                    AutoRetryOn429 = c.Safety.AutoRetryOn429,
                    MaxRetriesPerAccount = c.Safety.MaxRetriesPerAccount,
                    StopOnTokenError = c.Safety.StopOnTokenError
                },
                Proxy = new ProxySettings
                {
                    Enabled = c.Proxy.Enabled,
                    AutoAssign = c.Proxy.AutoAssign,
                    ShowProxyInfo = c.Proxy.ShowProxyInfo,
                    FallbackToDirect = c.Proxy.FallbackToDirect,
                    MaxRetriesPerProxy = c.Proxy.MaxRetriesPerProxy
                }
            };
        }
    }
}


