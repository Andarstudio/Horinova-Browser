using System;
using System.IO;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace Horinova
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Horinova");
            Directory.CreateDirectory(data);
            var s = new CefSettings { CachePath = Path.Combine(data, "cache") };
            Cef.Initialize(s);
            FilterEngine.Load(AppContext.BaseDirectory);
            new MainWindow().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Cef.Shutdown();
            base.OnExit(e);
        }
    }
}
