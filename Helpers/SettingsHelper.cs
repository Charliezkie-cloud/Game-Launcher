using Game_Launcher.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Game_Launcher.Helpers
{
    public static class SettingsHelper
    {
        public static readonly string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
        public static readonly Settings _defaultSettings = new Settings()
        {
            DarkMode = false,
            IconSize = 2
        };
        private static JsonSerializer jsonSerializer = new JsonSerializer()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        private static void SaveSettingsToFile(Settings settings)
        {
            using (StreamWriter streamWriter = File.CreateText(_settingsPath))
            {
                jsonSerializer.Serialize(streamWriter, settings);
            }
        }

        private static void CreateIfNotExists()
        {
            if (!File.Exists(_settingsPath))
                SaveSettingsToFile(_defaultSettings);
        }

        public static Settings GetSettings()
        {
            Settings appSettings = new Settings();

            CreateIfNotExists();

            using (StreamReader streamReader = new StreamReader(_settingsPath))
                appSettings = (Settings)jsonSerializer.Deserialize(streamReader, typeof(Settings));

            return appSettings;
        }

        public static Settings SetTheme(bool isDarkMode)
        {
            Settings appSettings = new Settings();

            CreateIfNotExists();

            using (StreamReader streamReader = new StreamReader(_settingsPath))
                appSettings = (Settings)jsonSerializer.Deserialize(streamReader, typeof(Settings));

            appSettings.DarkMode = isDarkMode;

            SaveSettingsToFile(appSettings);

            return appSettings;
        }

        public static Settings SetIconSize(int iconSize)
        {
            Settings appSettings = new Settings();

            CreateIfNotExists();

            using (StreamReader streamReader = new StreamReader(_settingsPath))
                appSettings = (Settings)jsonSerializer.Deserialize(streamReader, typeof(Settings));

            appSettings.IconSize = iconSize;

            SaveSettingsToFile(appSettings);

            return appSettings;
        }
    }
}
