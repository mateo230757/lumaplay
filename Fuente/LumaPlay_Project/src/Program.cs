using System;
using System.Windows.Forms;

namespace LumaPlay
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppSettings settings = AppSettings.Load();
            if (!settings.SetupComplete)
            {
                using (SetupForm setup = new SetupForm(settings))
                {
                    if (setup.ShowDialog() != DialogResult.OK) return;
                }
            }

            Application.Run(new MainForm(settings));
        }
    }
}
