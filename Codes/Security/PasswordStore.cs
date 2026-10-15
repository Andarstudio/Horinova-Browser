using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Horinova
{
    public class PasswordEntry
    {
        public string Site { get; set; }
        public string Username { get; set; }
        public string Secret { get; set; } // DPAPI-encrypted, base64
    }

    // Passwords are encrypted with Windows DPAPI (tied to the current Windows user).
    public static class PasswordStore
    {
        static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Horinova", "passwords.json");

        public static List<PasswordEntry> Load()
        {
            try { return JsonSerializer.Deserialize<List<PasswordEntry>>(File.ReadAllText(FilePath)) ?? new(); }
            catch { return new(); }
        }

        static void Save(List<PasswordEntry> list)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, JsonSerializer.Serialize(list));
        }

        public static void Add(string site, string user, string password)
        {
            var list = Load();
            var enc = ProtectedData.Protect(Encoding.UTF8.GetBytes(password), null, DataProtectionScope.CurrentUser);
            list.Add(new PasswordEntry { Site = site, Username = user, Secret = Convert.ToBase64String(enc) });
            Save(list);
        }

        public static void Remove(PasswordEntry e)
        {
            var list = Load();
            list.RemoveAll(x => x.Site == e.Site && x.Username == e.Username && x.Secret == e.Secret);
            Save(list);
        }

        public static string Reveal(PasswordEntry e)
        {
            var raw = ProtectedData.Unprotect(Convert.FromBase64String(e.Secret), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(raw);
        }
    }
}
