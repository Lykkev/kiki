using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kick_ChatBOT
{
    internal static class Program
    {
        private static System.Threading.Mutex singleInstanceMutex;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Impedir múltiples instancias por licencia/PC: mutex global por hardware
            var hw = Kick_ChatBOT.Services.LicenseService.ComputeHardwareId();
            bool created;
            singleInstanceMutex = new System.Threading.Mutex(true, "Global\\KickChatBot-" + hw, out created);
            if (!created)
            {
                MessageBox.Show("Ya hay una instancia en ejecución.", "Kick ChatBOT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
