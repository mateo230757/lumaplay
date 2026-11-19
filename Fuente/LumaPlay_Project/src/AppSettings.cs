using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LumaPlay
{
    public sealed class GameEntry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExecutablePath { get; set; }
        public bool IsBuiltIn { get; set; }
        public int LaunchCount { get; set; }
        public string LastPlayed { get; set; }

        public GameEntry()
        {
            Id = Guid.NewGuid().ToString("N");
            Name = "Nuevo juego";
            Description = "Juego añadido a tu biblioteca";
            ExecutablePath = "";
            LastPlayed = "Nunca";
        }
    }

    public sealed class AppSettings
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            WriteIndented = true
        };

        public string ProfileName { get; set; }
        public string Accent { get; set; }
        public bool SetupComplete { get; set; }
        public int BestScore { get; set; }
        public int TotalSessions { get; set; }
        public List<GameEntry> Games { get; set; }

        public AppSettings()
        {
            ProfileName = "Jugador";
            Accent = "Violeta";
            SetupComplete = false;
            BestScore = 0;
            TotalSessions = 0;
            Games = new List<GameEntry>();
        }

        public static string DataDirectory
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LumaPlay"); }
        }

        public static string SettingsFile
        {
            get { return Path.Combine(DataDirectory, "settings.json"); }
        }

        public static AppSettings Load()
        {
            AppSettings settings = new AppSettings();
            try
            {
                if (File.Exists(SettingsFile))
                {
                    AppSettings loaded = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsFile), JsonOptions);
                    if (loaded != null) settings = loaded;
                }
            }
            catch
            {
                settings = new AppSettings();
            }
            settings.Normalize();
            return settings;
        }

        public void Normalize()
        {
            if (String.IsNullOrWhiteSpace(ProfileName)) ProfileName = "Jugador";
            if (String.IsNullOrWhiteSpace(Accent)) Accent = "Violeta";
            if (Games == null) Games = new List<GameEntry>();
            Games = Games.Where(g => g != null).ToList();
            foreach (GameEntry game in Games)
            {
                if (String.IsNullOrWhiteSpace(game.Id)) game.Id = Guid.NewGuid().ToString("N");
                if (String.IsNullOrWhiteSpace(game.Name)) game.Name = "Juego sin nombre";
                if (String.IsNullOrWhiteSpace(game.Description)) game.Description = "Juego añadido a tu biblioteca";
                if (String.IsNullOrWhiteSpace(game.LastPlayed)) game.LastPlayed = "Nunca";
            }
            if (!Games.Any(g => g.Id == "neon-drift"))
            {
                Games.Insert(0, new GameEntry
                {
                    Id = "neon-drift",
                    Name = "Neon Drift",
                    Description = "Esquiva los obstáculos, cambia de carril y supera tu récord.",
                    ExecutablePath = "BUILT-IN",
                    IsBuiltIn = true,
                    LaunchCount = 0,
                    LastPlayed = "Nunca"
                });
            }
        }

        public void Save()
        {
            Normalize();
            Directory.CreateDirectory(DataDirectory);
            File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this, JsonOptions));
        }
    }
}
