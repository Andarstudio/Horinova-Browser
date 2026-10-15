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
            FilterEngine.TrackerBlock = TrackersCheck.IsChecked == true;
        }

        void ClearHistory_Click(object s, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete all browsing history?", "Clear history", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                HistoryStore.Clear();
        }

        void Add_Click(object s, RoutedEventArgs e)
        {
            if (SiteBox.Text.Trim().Length == 0 || PassBox.Password.Length == 0) return;
            PasswordStore.Add(SiteBox.Text.Trim(), UserBox.Text.Trim(), PassBox.Password);
            SiteBox.Clear(); UserBox.Clear(); PassBox.Clear();
            RefreshList();
        }

        void Reveal_Click(object s, RoutedEventArgs e)
        {
            if (PwList.SelectedItem is PasswordEntry p)
                MessageBox.Show($"{p.Site}\nUser: {p.Username}\nPassword: {PasswordStore.Reveal(p)}", "Saved password");
        }

        void Delete_Click(object s, RoutedEventArgs e)
        {
            if (PwList.SelectedItem is PasswordEntry p) { PasswordStore.Remove(p); RefreshList(); }
        }
    }
}
