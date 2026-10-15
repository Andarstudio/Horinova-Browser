using System;
using System.Collections.Generic;
using System.IO;

namespace Horinova
{
    public static class BlockStats
    {
        public static int Total;
        public static event Action Changed;
        public static void Hit() { Total++; Changed?.Invoke(); }
    }

    public static class FilterEngine
    {
        static readonly HashSet<string> ads = new(StringComparer.OrdinalIgnoreCase);
        static readonly HashSet<string> trackers = new(StringComparer.OrdinalIgnoreCase);
        public static bool AdBlock = true, TrackerBlock = true;

        public static void Load(string baseDir)
        {
            Read(Path.Combine(baseDir, "Assets", "filters", "ads.txt"), ads);
            Read(Path.Combine(baseDir, "Assets", "filters", "trackers.txt"), trackers);
        }

        static void Read(string path, HashSet<string> set)
        {
            if (!File.Exists(path)) return;
            foreach (var raw in File.ReadLines(path))
            {
                var l = raw.Trim();
                if (l.Length == 0 || l.StartsWith("#")) continue;
                var parts = l.Split(' ', '\t');
                set.Add(parts[parts.Length - 1].ToLowerInvariant()); // supports hosts-style lines too
            }
        }

        public static bool ShouldBlock(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var u)) return false;
            var host = u.Host;
            return (AdBlock && Match(host, ads)) || (TrackerBlock && Match(host, trackers));
        }

        // matches example.com and any subdomain of it
        static bool Match(string host, HashSet<string> set)
        {
            while (true)
            {
                if (set.Contains(host)) return true;
                int i = host.IndexOf('.');
                if (i < 0) return false;
                host = host.Substring(i + 1);
            }
        }
    }
}
