using System.IO;
using System.Text.Json;

namespace PdfSearch.Services
{
    internal class AppConfigService
    {
        private const string _configFileName = "config.json";

        public AppConfigService()
        {
            Config = Load();
        }
        public AppConfig Config { get; private set; }
        public void Update(Action<AppConfig> updateAction)
        {
            var config = Config;
            updateAction(config);
            Save(config);
        }

        private AppConfig Load()
        {
            var configFile = GetConfigFilePath();
            if (!File.Exists(configFile))
            {
                return new AppConfig();
            }

            try
            {
                using (var file = File.OpenText(configFile))
                {
                    var json = file.ReadToEnd();
                    return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                }
            }
            catch
            {
                return new AppConfig();
            }
        }
      
        private void Save(AppConfig config)
        {
            EnsureConfigDirCreated();
            using (var file = File.CreateText(GetConfigFilePath()))
            {
                var json = JsonSerializer.Serialize(config);
                file.Write(json);
            }
            Config = config;
        }

        private string GetConfigDir()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PdfSearch");
        }

        private string GetConfigFilePath()
        {
            return Path.Combine(GetConfigDir(), _configFileName);
        }

        private void EnsureConfigDirCreated()
        {
            var configDir = GetConfigDir();
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
        }
    }
}
