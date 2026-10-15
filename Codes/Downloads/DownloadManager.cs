using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using CefSharp;

namespace Horinova
{
    public class DownloadRow : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public IDownloadItemCallback Callback { get; set; }

        int percent; string status = "Starting..."; bool active = true;
        public int Percent { get => percent; set { percent = value; Raise(nameof(Percent)); } }
        public string Status { get => status; set { status = value; Raise(nameof(Status)); } }
        public bool Active { get => active; set { active = value; Raise(nameof(Active)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        void Raise(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public static class DownloadManager
    {
        public static ObservableCollection<DownloadRow> Items { get; } = new();
        public static event Action Changed;

        public static string DownloadsFolder
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        // Returns a path that doesn't overwrite an existing file: name.ext, name (1).ext, ...
        public static string UniquePath(string fileName)
        {
            fileName = string.Concat(fileName.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
            if (string.IsNullOrWhiteSpace(fileName)) fileName = "download";
            var path = Path.Combine(DownloadsFolder, fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            for (int i = 1; File.Exists(path); i++)
                path = Path.Combine(DownloadsFolder, $"{name} ({i}){ext}");
            return path;
        }

        public static void OnUi(Action a)
        {
            var d = Application.Current?.Dispatcher;
            if (d == null) return;
            d.BeginInvoke(a);
        }

        public static void Raise() => Changed?.Invoke();
    }

    public class HorinovaDownloadHandler : IDownloadHandler
    {
        public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod) => true;

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem item, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    var path = DownloadManager.UniquePath(item.SuggestedFileName);
                    var row = new DownloadRow
                    {
                        Id = item.Id,
                        Url = item.Url,
                        FileName = Path.GetFileName(path),
                        FullPath = path
                    };
                    DownloadManager.OnUi(() => { DownloadManager.Items.Insert(0, row); DownloadManager.Raise(); });
                    callback.Continue(path, showDialog: false);
                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem item, IDownloadItemCallback callback)
        {
            DownloadManager.OnUi(() =>
            {
                var row = DownloadManager.Items.FirstOrDefault(r => r.Id == item.Id && r.Active);
                if (row == null) return;
                row.Callback = callback;
                row.Percent = item.PercentComplete;
                if (item.IsComplete) { row.Percent = 100; row.Status = "Done"; row.Active = false; row.Callback = null; }
                else if (item.IsCancelled) { row.Status = "Cancelled"; row.Active = false; row.Callback = null; }
                else row.Status = $"{item.PercentComplete}%  -  {item.CurrentSpeed / 1024} KB/s";
                DownloadManager.Raise();
            });
        }
    }
}
