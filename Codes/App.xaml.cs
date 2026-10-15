using System;
using System.IO;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace Horinova
{
    public partial class App : Application
    {
        static readonly string DataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Horinova");

        static string LogPath => Path.Combine(DataDir, "crash.log");

        static void Log(string text)
        {
            try
            {
                Directory.CreateDirectory(DataDir);
                File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}\r\n\r\n");
            }
            catch { }
        }

        public App()
        {
            DispatcherUnhandledException += (s, e) =>
            {
                Log(e.Exception.ToString());
                MessageBox.Show(e.Exception.Message + "\n\nDetails saved to:\n" + LogPath, "Horinova error");
                e.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log(e.ExceptionObject?.ToString());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                Directory.CreateDirectory(DataDir);
                var settings = new CefSettings
                {
                    CachePath = Path.Combine(DataDir, "cache"),
                    LogFile = Path.Combine(DataDir, "cef.log"),
                    LogSeverity = LogSeverity.Warning
                };

                if (!Cef.Initialize(settings))
                    throw new InvalidOperationException("The Chromium engine failed to start (Cef.Initialize returned false).");

                FilterEngine.Load(AppContext.BaseDirectory);
                new MainWindow().Show();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                MessageBox.Show(ex.Message + "\n\nDetails saved to:\n" + LogPath, "Horinova could not start");
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try { Cef.Shutdown(); } catch { }
            base.OnExit(e);
        }
    }
}
