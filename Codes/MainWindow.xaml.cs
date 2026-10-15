using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;

namespace Horinova
{
    public partial class MainWindow : Window
    {
        // Shared in-memory context for all private tabs: no cookies, cache or logins are written to disk.
        // It is discarded when the app closes.
        static readonly IRequestContext PrivateContext = new RequestContext(new RequestContextSettings());

        public MainWindow()
        {
            InitializeComponent();
            BlockStats.Changed += () => Dispatcher.Invoke(() => ShieldText.Text = $"🛡 {BlockStats.Total}");
            NewTab("https://www.google.com");
        }

        ChromiumWebBrowser Current => (Tabs.SelectedItem as TabItem)?.Content as ChromiumWebBrowser;

        void NewTab(string url, bool isPrivate = false)
        {
            var b = new ChromiumWebBrowser(url) { RequestHandler = new BlockRequestHandler(), DownloadHandler = new HorinovaDownloadHandler() };
            if (isPrivate) b.RequestContext = PrivateContext;

            var prefix = isPrivate ? "🕶 " : "";
            var header = new TextBlock { Text = prefix + "New tab", MaxWidth = 150, TextTrimming = TextTrimming.CharacterEllipsis };
            var tab = new TabItem { Header = header, Content = b };

            b.TitleChanged += (s, e) => Dispatcher.Invoke(() => header.Text = prefix + (e.NewValue as string ?? "New tab"));
            b.AddressChanged += (s, e) => Dispatcher.Invoke(() => { if (Current == b) AddressBox.Text = e.NewValue as string; });

            // Private tabs are never added to history.
            if (!isPrivate)
            {
                b.FrameLoadEnd += (s, e) =>
                {
                    if (!e.Frame.IsMain) return;
                    Dispatcher.Invoke(() => HistoryStore.Add(b.Title, e.Url));
                };
            }

            Tabs.Items.Add(tab);
            Tabs.SelectedItem = tab;
        }

        void Tabs_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (e.Source == Tabs && Current != null) AddressBox.Text = Current.Address;
        }

        void Address_KeyDown(object s, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || Current == null) return;
            var t = AddressBox.Text.Trim();
            if (t.Length == 0) return;
            if (!t.Contains("://"))
                t = (t.Contains('.') && !t.Contains(' ')) ? "https://" + t : "https://www.google.com/search?q=" + Uri.EscapeDataString(t);
            Current.Load(t);
        }

        void Back_Click(object s, RoutedEventArgs e) { if (Current?.CanGoBack == true) Current.Back(); }
        void Forward_Click(object s, RoutedEventArgs e) { if (Current?.CanGoForward == true) Current.Forward(); }
        void Reload_Click(object s, RoutedEventArgs e) => Current?.Reload();
        void NewTab_Click(object s, RoutedEventArgs e) => NewTab("https://www.google.com");
        void PrivateTab_Click(object s, RoutedEventArgs e) => NewTab("https://www.google.com", true);
        void History_Click(object s, RoutedEventArgs e) => new HistoryWindow(url => NewTab(url)) { Owner = this }.Show();
        void Downloads_Click(object s, RoutedEventArgs e) => new DownloadsWindow { Owner = this }.Show();
        void Settings_Click(object s, RoutedEventArgs e) => new SettingsWindow { Owner = this }.ShowDialog();
    }
}
