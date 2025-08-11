namespace Kick_ChatBOT
{
    partial class ChatBOT
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.txtChannel = new MetroFramework.Controls.MetroTextBox();
            this.btnStart = new MetroFramework.Controls.MetroButton();
            this.btnStartRotating = new MetroFramework.Controls.MetroButton();
            this.btnStop = new MetroFramework.Controls.MetroButton();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.btnLoadKicks = new MetroFramework.Controls.MetroButton();
            this.btnLoadProxies = new MetroFramework.Controls.MetroButton();
            this.btnLoadMessages = new MetroFramework.Controls.MetroButton();
            this.btnLoadConfig = new MetroFramework.Controls.MetroButton();
            this.toggleProxy = new MetroFramework.Controls.MetroToggle();
            this.lblProxy = new MetroFramework.Controls.MetroLabel();
            this.txtManualMessage = new MetroFramework.Controls.MetroTextBox();
            this.btnSendManual = new MetroFramework.Controls.MetroButton();
            this.btnLoadCategoryMessages = new MetroFramework.Controls.MetroButton();
            this.btnLoadGreetings = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // metroButton1
            // 
            this.metroButton1.Location = new System.Drawing.Point(0, 0);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(75, 23);
            this.metroButton1.TabIndex = 0;
            this.metroButton1.UseSelectable = true;
            // 
            // txtChannel
            // 
            // 
            // 
            // 
            this.txtChannel.CustomButton.Image = null;
            this.txtChannel.CustomButton.Location = new System.Drawing.Point(231, 1);
            this.txtChannel.CustomButton.Name = "";
            this.txtChannel.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.txtChannel.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtChannel.CustomButton.TabIndex = 1;
            this.txtChannel.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtChannel.CustomButton.UseSelectable = true;
            this.txtChannel.CustomButton.Visible = false;
            this.txtChannel.Lines = new string[0];
            this.txtChannel.Location = new System.Drawing.Point(23, 81);
            this.txtChannel.MaxLength = 10;
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.PasswordChar = '\0';
            this.txtChannel.PromptText = "Canal";
            this.txtChannel.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtChannel.SelectedText = "";
            this.txtChannel.SelectionLength = 0;
            this.txtChannel.SelectionStart = 0;
            this.txtChannel.ShortcutsEnabled = true;
            this.txtChannel.Size = new System.Drawing.Size(253, 23);
            this.txtChannel.Style = MetroFramework.MetroColorStyle.Green;
            this.txtChannel.TabIndex = 9;
            this.txtChannel.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.txtChannel.UseSelectable = true;
            this.txtChannel.WaterMark = "Canal";
            this.txtChannel.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtChannel.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(300, 81);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(129, 23);
            this.btnStart.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnStart.TabIndex = 11;
            this.btnStart.Text = "Iniciar";
            this.btnStart.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnStart.UseSelectable = true;
            // 
            // btnStartRotating
            // 
            this.btnStartRotating.Location = new System.Drawing.Point(456, 81);
            this.btnStartRotating.Name = "btnStartRotating";
            this.btnStartRotating.Size = new System.Drawing.Size(129, 23);
            this.btnStartRotating.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnStartRotating.TabIndex = 12;
            this.btnStartRotating.Text = "Rotativo";
            this.btnStartRotating.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnStartRotating.UseSelectable = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(609, 81);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(129, 23);
            this.btnStop.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnStop.TabIndex = 13;
            this.btnStop.Text = "Detener";
            this.btnStop.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnStop.UseSelectable = true;
            // 
            // lstLog
            // 
            this.lstLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.lstLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLog.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(23, 160);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(715, 273);
            this.lstLog.TabIndex = 14;
            // 
            // btnLoadKicks
            // 
            this.btnLoadKicks.Location = new System.Drawing.Point(23, 126);
            this.btnLoadKicks.Name = "btnLoadKicks";
            this.btnLoadKicks.Size = new System.Drawing.Size(129, 23);
            this.btnLoadKicks.Style = MetroFramework.MetroColorStyle.Blue;
            this.btnLoadKicks.TabIndex = 15;
            this.btnLoadKicks.Text = "Cargar kicks.json";
            this.btnLoadKicks.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnLoadKicks.UseSelectable = true;
            // 
            // btnLoadProxies
            // 
            this.btnLoadProxies.Location = new System.Drawing.Point(168, 126);
            this.btnLoadProxies.Name = "btnLoadProxies";
            this.btnLoadProxies.Size = new System.Drawing.Size(129, 23);
            this.btnLoadProxies.Style = MetroFramework.MetroColorStyle.Blue;
            this.btnLoadProxies.TabIndex = 16;
            this.btnLoadProxies.Text = "Cargar proxies.json";
            this.btnLoadProxies.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnLoadProxies.UseSelectable = true;
            // 
            // btnLoadMessages
            // 
            this.btnLoadMessages.Location = new System.Drawing.Point(313, 126);
            this.btnLoadMessages.Name = "btnLoadMessages";
            this.btnLoadMessages.Size = new System.Drawing.Size(146, 23);
            this.btnLoadMessages.Style = MetroFramework.MetroColorStyle.Blue;
            this.btnLoadMessages.TabIndex = 17;
            this.btnLoadMessages.Text = "Cargar messages.json";
            this.btnLoadMessages.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnLoadMessages.UseSelectable = true;
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Location = new System.Drawing.Point(475, 126);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(129, 23);
            this.btnLoadConfig.Style = MetroFramework.MetroColorStyle.Blue;
            this.btnLoadConfig.TabIndex = 18;
            this.btnLoadConfig.Text = "Cargar config.json";
            this.btnLoadConfig.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnLoadConfig.UseSelectable = true;
            // 
            // toggleProxy
            // 
            this.toggleProxy.AutoSize = true;
            this.toggleProxy.Checked = true;
            this.toggleProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleProxy.Location = new System.Drawing.Point(628, 129);
            this.toggleProxy.Name = "toggleProxy";
            this.toggleProxy.Size = new System.Drawing.Size(80, 17);
            this.toggleProxy.Style = MetroFramework.MetroColorStyle.Green;
            this.toggleProxy.TabIndex = 19;
            this.toggleProxy.Text = "On";
            this.toggleProxy.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.toggleProxy.UseSelectable = true;
            // 
            // lblProxy
            // 
            this.lblProxy.AutoSize = true;
            this.lblProxy.Location = new System.Drawing.Point(628, 107);
            this.lblProxy.Name = "lblProxy";
            this.lblProxy.Size = new System.Drawing.Size(81, 19);
            this.lblProxy.TabIndex = 20;
            this.lblProxy.Text = "Usar Proxies";
            this.lblProxy.Theme = MetroFramework.MetroThemeStyle.Dark;
            //
            // txtManualMessage
            //
            this.txtManualMessage.CustomButton.Image = null;
            this.txtManualMessage.CustomButton.Location = new System.Drawing.Point(231, 1);
            this.txtManualMessage.CustomButton.Name = "";
            this.txtManualMessage.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.txtManualMessage.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.txtManualMessage.CustomButton.TabIndex = 1;
            this.txtManualMessage.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.txtManualMessage.CustomButton.UseSelectable = true;
            this.txtManualMessage.CustomButton.Visible = false;
            this.txtManualMessage.Lines = new string[0];
            this.txtManualMessage.Location = new System.Drawing.Point(23, 52);
            this.txtManualMessage.MaxLength = 32767;
            this.txtManualMessage.Name = "txtManualMessage";
            this.txtManualMessage.PasswordChar = '\0';
            this.txtManualMessage.PromptText = "Mensaje manual";
            this.txtManualMessage.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.txtManualMessage.SelectedText = "";
            this.txtManualMessage.SelectionLength = 0;
            this.txtManualMessage.SelectionStart = 0;
            this.txtManualMessage.ShortcutsEnabled = true;
            this.txtManualMessage.Size = new System.Drawing.Size(253, 23);
            this.txtManualMessage.Style = MetroFramework.MetroColorStyle.Green;
            this.txtManualMessage.TabIndex = 21;
            this.txtManualMessage.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.txtManualMessage.UseSelectable = true;
            this.txtManualMessage.WaterMark = "Mensaje manual";
            this.txtManualMessage.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.txtManualMessage.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            //
            // btnSendManual
            //
            this.btnSendManual.Location = new System.Drawing.Point(300, 52);
            this.btnSendManual.Name = "btnSendManual";
            this.btnSendManual.Size = new System.Drawing.Size(129, 23);
            this.btnSendManual.Style = MetroFramework.MetroColorStyle.Teal;
            this.btnSendManual.TabIndex = 22;
            this.btnSendManual.Text = "Enviar manual";
            this.btnSendManual.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnSendManual.UseSelectable = true;
            //
            // btnLoadCategoryMessages
            //
            this.btnLoadCategoryMessages.Location = new System.Drawing.Point(23, 155);
            this.btnLoadCategoryMessages.Name = "btnLoadCategoryMessages";
            this.btnLoadCategoryMessages.Size = new System.Drawing.Size(183, 23);
            this.btnLoadCategoryMessages.Style = MetroFramework.MetroColorStyle.Blue;
            this.btnLoadCategoryMessages.TabIndex = 23;
            this.btnLoadCategoryMessages.Text = "Cargar category_messages.json";
            this.btnLoadCategoryMessages.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnLoadCategoryMessages.UseSelectable = true;
            //
            // btnLoadGreetings
            //
            this.btnLoadGreetings.Location = new System.Drawing.Point(224, 155);
            this.btnLoadGreetings.Name = "btnLoadGreetings";
            this.btnLoadGreetings.Size = new System.Drawing.Size(183, 23);
            this.btnLoadGreetings.Style = MetroFramework.MetroColorStyle.Blue;
            this.btnLoadGreetings.TabIndex = 24;
            this.btnLoadGreetings.Text = "Cargar greetings.json";
            this.btnLoadGreetings.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnLoadGreetings.UseSelectable = true;
            // 
            // ChatBOT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 450);
            this.Controls.Add(this.btnLoadGreetings);
            this.Controls.Add(this.btnLoadCategoryMessages);
            this.Controls.Add(this.btnSendManual);
            this.Controls.Add(this.txtManualMessage);
            this.Controls.Add(this.lblProxy);
            this.Controls.Add(this.toggleProxy);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.btnLoadMessages);
            this.Controls.Add(this.btnLoadProxies);
            this.Controls.Add(this.btnLoadKicks);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStartRotating);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtChannel);
            this.MaximizeBox = false;
            this.Name = "ChatBOT";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Green;
            this.Text = "KICK by Savage";
            this.TextAlign = MetroFramework.Forms.MetroFormTextAlign.Center;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.ChatBOT_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroButton metroButton1;
        private MetroFramework.Controls.MetroTextBox txtChannel;
        private MetroFramework.Controls.MetroButton btnStart;
        private MetroFramework.Controls.MetroButton btnStartRotating;
        private MetroFramework.Controls.MetroButton btnStop;
        private System.Windows.Forms.ListBox lstLog;
        private MetroFramework.Controls.MetroButton btnLoadKicks;
        private MetroFramework.Controls.MetroButton btnLoadProxies;
        private MetroFramework.Controls.MetroButton btnLoadMessages;
        private MetroFramework.Controls.MetroButton btnLoadConfig;
        private MetroFramework.Controls.MetroToggle toggleProxy;
        private MetroFramework.Controls.MetroLabel lblProxy;
        private MetroFramework.Controls.MetroTextBox txtManualMessage;
        private MetroFramework.Controls.MetroButton btnSendManual;
        private MetroFramework.Controls.MetroButton btnLoadCategoryMessages;
        private MetroFramework.Controls.MetroButton btnLoadGreetings;
    }
}

