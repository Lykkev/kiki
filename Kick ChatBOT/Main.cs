using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kick_ChatBOT.Services;

namespace Kick_ChatBOT
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {
        private LicenseService licenseService;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Inicializar servicio de licencias con tu cadena Mongo
            var mongo = "mongodb+srv://cursed:1234@cluster0.anibcke.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            licenseService = new LicenseService(mongo);
            // Ejecutar auto-login después de que el formulario se haya mostrado para evitar cerrar en Load
            this.Shown += Main_Shown;
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            try
            {
                var cached = SecureLocalCache.LoadBoundKey();
                if (cached == null) return;
                var currentHw = LicenseService.ComputeHardwareId();
                if (!string.Equals(cached.Item2, currentHw, StringComparison.OrdinalIgnoreCase)) return;

                // Validar sin bloquear UI
                await Task.Run(() => licenseService.ValidateAndBind(cached.Item1));

                // Iniciar heartbeat (15 min) con gracia offline (24h)
                var hw = LicenseService.ComputeHardwareId();
                var hb = new HeartbeatService(licenseService, cached.Item1, hw, TimeSpan.FromMinutes(15), TimeSpan.FromHours(24));
                hb.OnInvalid += msg =>
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show(this, msg, "Licencia inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }));
                    }
                    catch { }
                };
                hb.Start();

                // Cambiar a la app principal sin usar ShowDialog ni cerrar durante Load
                this.Hide();
                var form = new ChatBOT();
                form.FormClosed += (s, ev) => { hb.Dispose(); this.Close(); };
                form.Show();
            }
            catch
            {
                // si falla el auto-login, permanecemos en el login
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Validando licencia...";
            lblStatus.ForeColor = System.Drawing.Color.Gold;
            btnLogin.Enabled = false;
            try
            {
                // Validación sin bloqueo de UI
                var key = txtKey.Text?.Trim();
                if (string.IsNullOrEmpty(key)) { lblStatus.Text = "Ingresa la licencia"; return; }

                string error = null; LicenseInfo lic = null;
                await System.Threading.Tasks.Task.Run(() =>
                {
                    if (!licenseService.TryValidateAndBind(key, out lic, out error))
                    {
                        return;
                    }
                    // Guardar caché local ligada al hardware
                    SecureLocalCache.SaveBoundKey(key, lic.HardwareId ?? LicenseService.ComputeHardwareId());
                });

                if (!string.IsNullOrEmpty(error))
                {
                    lblStatus.Text = "❌ " + error;
                    lblStatus.ForeColor = System.Drawing.Color.IndianRed;
                    MessageBox.Show(this, error, "Licencia inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblStatus.Text = "✅ Licencia válida. Cargando...";
                // Arrancar heartbeat (15 min) con gracia offline 24h
                var hw = LicenseService.ComputeHardwareId();
                var hb = new HeartbeatService(licenseService, lic.Key, hw, TimeSpan.FromMinutes(15), TimeSpan.FromHours(24));
                hb.OnInvalid += msg =>
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show(this, msg, "Licencia inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }));
                    }
                    catch { }
                };
                hb.Start();

                // Abrir app principal sin bloquear y cerrar login al cerrar principal
                this.Hide();
                var form = new ChatBOT();
                form.FormClosed += (s2, e2) => { hb.Dispose(); this.Close(); };
                form.Show();
            }
            catch (System.TimeoutException tex)
            {
                lblStatus.Text = "⏱️ No se pudo contactar con el servidor de licencias.";
                lblStatus.ForeColor = System.Drawing.Color.IndianRed;
                MessageBox.Show(this, tex.Message, "Tiempo de espera", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException ioe)
            {
                // Errores funcionales de licencia: no existe, expirada, revocada, enlazada a otro equipo
                lblStatus.Text = "❌ " + ioe.Message;
                lblStatus.ForeColor = System.Drawing.Color.IndianRed;
                MessageBox.Show(this, ioe.Message, "Licencia inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.Exception ex)
            {
                lblStatus.Text = "❌ " + ex.Message;
                lblStatus.ForeColor = System.Drawing.Color.IndianRed;
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}
