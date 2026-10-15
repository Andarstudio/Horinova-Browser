using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Horinova
{
    public static class UpdateChecker
    {
        public static async Task<string> CheckAsync()
        {
            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
                var json = await http.GetStringAsync(AppInfo.UpdateUrl);
                var latest = JsonDocument.Parse(json).RootElement.GetProperty("version").GetString();
                return Version.Parse(latest) > Version.Parse(AppInfo.Version)
                    ? $"Update available: {latest} (you have {AppInfo.Version})"
                    : $"Horinova is up to date ({AppInfo.Version})";
            }
            catch
            {
                return "Could not check for updates (no connection or update URL not set).";
            }
        }
    }
}
