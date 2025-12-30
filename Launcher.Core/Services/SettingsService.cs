using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;

namespace Launcher.Services
{
    public class SettingsService
    {
        private static string Filepath = "Settings/Setting.json"; 
        public Settings.Settings Settings { get; set; } = new Settings.Settings();

        public void GetSettings()
        {
            if (!File.Exists(Filepath))
            {
                Debug.WriteLine("No Settingsfile found");
                Settings = CreateDefaultSettings();
                SaveSettings();
            }
            else
            {
                Debug.WriteLine("Loading Settings from " + Filepath);
                string fileString = File.ReadAllText(Filepath);
                Settings = JsonSerializer.Deserialize<Settings.Settings>(fileString) ?? Settings;
            }
        }

        private Settings.Settings CreateDefaultSettings()
        {
            return new Settings.DefaultSettings();
        }

        public void SaveSettings()
        {
            JsonSerializerOptions JsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string _fileString = JsonSerializer.Serialize(Settings, JsonOptions);
            File.WriteAllText(Filepath, _fileString);
        }
    }
}
