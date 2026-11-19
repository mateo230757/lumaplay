using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LumaPlay
{
    internal static class Program
    {
        private static string LogFile
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "LumaPlay",
                    "startup.log");
            }
        }

        [STAThread]
        private static void Main()
        {
            WriteLog("--------------------------------------------------");
            WriteLog("LumaPlay starting. Version 1.1.1");
            WriteLog("OS: " + Environment.OSVersion.VersionString);
            WriteLog("64-bit OS: " + Environment.Is64BitOperatingSystem);
            WriteLog("64-bit process: " + Environment.Is64BitProcess);
            WriteLog("CLR: " + Environment.Version);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                WriteLog("Loading local settings.");
                AppSettings settings = AppSettings.Load();
                settings.Normalize();
                WriteLog("Settings loaded. Games: " + settings.Games.Count);

                if (!settings.SetupComplete)
                {
                    WriteLog("Opening first-run profile setup.");
                    using (SetupForm setup = new SetupForm(settings))
                    {
                        if (setup.ShowDialog() != DialogResult.OK)
                        {
                            WriteLog("First-run setup was cancelled.");
                            return;
                        }
                    }
                    WriteLog("First-run profile setup completed.");
                }

                WriteLog("Creating main window.");
                using (MainForm main = new MainForm(settings))
                {
                    WriteLog("Entering Windows Forms message loop.");
                    Application.Run(main);
                }
                WriteLog("LumaPlay closed normally.");
            }
            catch (Exception ex)
            {
                WriteException("Fatal startup error", ex);
                ShowError("LumaPlay no pudo iniciarse.", ex);
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            WriteException("Windows Forms thread exception", e.Exception);
            ShowError("LumaPlay encontró un error.", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
                WriteException("Unhandled application exception", ex);
            else
                WriteLog("Unhandled non-Exception error: " + Convert.ToString(e.ExceptionObject));
        }

        private static void ShowError(string heading, Exception ex)
        {
            string message = heading + Environment.NewLine + Environment.NewLine
                + ex.GetType().FullName + ": " + ex.Message
                + Environment.NewLine + Environment.NewLine
                + "Se guardaron los detalles en:"
                + Environment.NewLine + LogFile;
            try
            {
                MessageBox.Show(message, "LumaPlay — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                // Logging must remain available even if Windows cannot display the dialog.
            }
        }

        private static void WriteException(string heading, Exception ex)
        {
            WriteLog(heading);
            WriteLog(ex.ToString());
        }

        private static void WriteLog(string message)
        {
            try
            {
                string directory = Path.GetDirectoryName(LogFile);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                File.AppendAllText(LogFile,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + message
                    + Environment.NewLine,
                    Encoding.UTF8);
            }
            catch
            {
                // Never let logging prevent the application from starting.
            }
        }
    }
}
