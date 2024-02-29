using System;
using System.Configuration;
using System.Globalization;

namespace Neo4JHTTPBrowser.Helpers
{
    internal static class AppConfigHelper
    {
        public static string Neo4JBaseUrl
        {
            get
            {
                return GetUrlValue(nameof(Neo4JBaseUrl));
            }
            set
            {
                SaveConfig(nameof(Neo4JBaseUrl), value.Trim());
            }
        }

        public static bool Neo4JVerifySsl
        {
            get
            {
                if (bool.TryParse(ConfigurationManager.AppSettings[nameof(Neo4JVerifySsl)], out bool enabled))
                {
                    return enabled;
                }
                return false;
            }
            set
            {
                SaveConfig(nameof(Neo4JVerifySsl), value.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static string GetUrlValue(string key)
        {
            var url = ConfigurationManager.AppSettings[key].Trim();

            // Remove the last splash character.
            if (url.EndsWith("/", StringComparison.InvariantCulture))
            {
                url = url.Substring(0, url.Length - 1);
            }

            // Remove the last question character.
            if (url.EndsWith("?", StringComparison.InvariantCulture))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        private static void SaveConfig(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
    }
}
