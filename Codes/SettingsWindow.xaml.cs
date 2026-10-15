using System.Linq;
using System.Windows;
using CefSharp;

namespace Horinova
{
    public partial class SettingsWindow : Window
    {
        bool loading = true;

        public SettingsWindow()
        {
            InitializeComponent();
            AdsCheck.IsChecked = FilterEngine.AdBlock;
            TrackersCheck.IsChecked = FilterEngine.TrackerBlock;
            VerText.Text = $"Version {AppInfo.Version}";
            ChromiumText.Text = $"Chromium {Cef.ChromiumVersion}";
            RefreshList();
            loading = false;
        }

        void RefreshList() => PwList.ItemsSource = PasswordStore.Load();

        async void CheckUpdate_Click(object s, RoutedEventArgs e)
        {
            UpdateText.Text = "Checking...";
            UpdateText.Text = await UpdateChecker.CheckAsync();
        }

        void Block_Changed(object s, RoutedEventArgs e)
        {
            if (loading) return;
            FilterEngine.AdBlock = AdsCheck.IsChecked == true;
