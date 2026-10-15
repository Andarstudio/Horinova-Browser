using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Horinova
{
    public partial class HistoryWindow : Window
    {
        readonly Action<string> open;

        public HistoryWindow(Action<string> openUrl)
        {
            InitializeComponent();
            open = openUrl;
            Refresh();
        }

        void Refresh()
        {
            var q = SearchBox.Text.Trim();
            var items = HistoryStore.Load().OrderByDescending(h => h.Time)
                .Where(h => q.Length == 0 ||
                            (h.Title ?? "").Contains(q, StringComparison.OrdinalIgnoreCase) ||
                            h.Url.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            List.ItemsSource = items;
        }

        void Search_Changed(object s, System.Windows.Controls.TextChangedEventArgs e) => Refresh();

        void List_DoubleClick(object s, MouseButtonEventArgs e)
        {
            if (List.SelectedItem is HistoryEntry h) open(h.Url);
        }

        void Delete_Click(object s, RoutedEventArgs e)
        {
            foreach (var h in List.SelectedItems.Cast<HistoryEntry>().ToList()) HistoryStore.Remove(h);
            Refresh();
        }

        void Clear_Click(object s, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete all browsing history?", "Clear history", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            { HistoryStore.Clear(); Refresh(); }
        }
    }
}
