using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Horinova
{
    public class HistoryEntry
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime Time { get; set; }
        public string TimeText => Time.ToString("yyyy-MM-dd HH:mm");
    }

    public static class HistoryStore
    {
        static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Horinova", "history.json");
        static readonly object gate = new();

        public static List<HistoryEntry> Load()
        {
            lock (gate)
            {
                try { return JsonSerializer.Deserialize<List<HistoryEntry>>(File.ReadAllText(FilePath)) ?? new(); }
                catch { return new(); }
            }
        }

        static void Save(List<HistoryEntry> list)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, JsonSerializer.Serialize(list));
        }

        public static void Add(string title, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !(url.StartsWith("http://") || url.StartsWith("https://"))) return;
            lock (gate)
            {
                var list = Load();
                list.Add(new HistoryEntry { Title = string.IsNullOrWhiteSpace(title) ? url : title, Url = url, Time = DateTime.Now });
                if (list.Count > 5000) list = list.Skip(list.Count - 5000).ToList();
                Save(list);
            }
        }

        public static void Remove(HistoryEntry e)
        {
            lock (gate)
            {
                var list = Load();
                list.RemoveAll(x => x.Url == e.Url && x.Time == e.Time);
                Save(list);
            }
        }

        public static void Clear() { lock (gate) { Save(new List<HistoryEntry>()); } }
    }
}
