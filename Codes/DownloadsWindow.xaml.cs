using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace Horinova
{
    public partial class DownloadsWindow : Window
    {
        public DownloadsWindow()
        {
            InitializeComponent();
            List.ItemsSource = DownloadManager.Items;
        }

        DownloadRow Sel => List.SelectedItem as DownloadRow;

        void Open_Click(object s, RoutedEventArgs e)
        {
            if (Sel != null && !Sel.Active && File.Exists(Sel.FullPath))
                Process.Start(new ProcessStartInfo(Sel.FullPath) { UseShellExecute = true });
        }

        void Folder_Click(object s, RoutedEventArgs e)
        {
            if (Sel != null && File.Exists(Sel.FullPath))
                Process.Start("explorer.exe", $"/select,\"{Sel.FullPath}\"");
            else
                Process.Start(new ProcessStartInfo(DownloadManager.DownloadsFolder) { UseShellExecute = true });
        }

        void Cancel_Click(object s, RoutedEventArgs e) => Sel?.Callback?.Cancel();

        void Clear_Click(object s, RoutedEventArgs e)
        {
            foreach (var r in DownloadManager.Items.Where(r => !r.Active).ToList()) DownloadManager.Items.Remove(r);
            DownloadManager.Raise();
        }
    }
}
