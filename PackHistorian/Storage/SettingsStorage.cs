using Hearthstone_Deck_Tracker;
using Newtonsoft.Json;
using System.IO;
using System.Xml;

namespace PackTracker.Storage
{
    internal class SettingsStorage : ISettingsStorage
    {
        public Settings Fetch()
        {
            var Settings = new Settings();

            var path = Path.Combine(Config.AppDataPath, "PackTracker", "Settings.json");
            try
            {
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(path));
            }
            catch
            {
                // Supress
            }

            path = Path.Combine(Config.AppDataPath, "PackTracker", "Settings.xml");
            if (File.Exists(path))
            {
                var Xml = new XmlDocument();
                Xml.Load(path);
                var Root = Xml.SelectSingleNode("settings");

                if (Root != null)
                {
                    if (bool.TryParse(Root.SelectSingleNode("spoil")?.InnerText, out var spoil))
                    {
                        Settings.Spoil = spoil;
                    }

                    if (bool.TryParse(Root.SelectSingleNode("pityoverlay")?.InnerText, out var pityoverlay))
                    {
                        Settings.PityOverlay = pityoverlay;
                    }

                    if (bool.TryParse(Root.SelectSingleNode("update")?.InnerText, out var update))
                    {
                        Settings.Update = update;
                    }
                }
            }

            return Settings;
        }

        public void Store(Settings Settings)
        {
            var path = Path.Combine(Config.AppDataPath, "PackTracker");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(Path.Combine(path, "Settings.json"), JsonConvert.SerializeObject(Settings, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
