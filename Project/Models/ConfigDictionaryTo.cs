using System.Collections.Generic;

namespace FlexibleDBMS
{
    public static class ConfigDictionaryTo
    {
        public static ISQLConnectionSettings ToISQLConnectionSettings(this IDictionary<string, object> config)
        {
            ISQLConnectionSettings data = new SQLConnectionSettings();
            if (!(config?.Count > 0))
                return data;

            data.Database = config[nameof(ISQLConnectionSettings.Database)]?.ToString();
            data.Table = config[nameof(ISQLConnectionSettings.Table)]?.ToString();
            data.ProviderName = config[nameof(ISQLConnectionSettings.ProviderName)]?.ToString().GetSQLProvider();
            data.Host = config[nameof(ISQLConnectionSettings.Host)]?.ToString();
            data.Port = int.TryParse(config[nameof(ISQLConnectionSettings.Port)]?.ToString(), out int port) ? port : 0;
            data.Username = config[nameof(ISQLConnectionSettings.Username)]?.ToString();
            data.Password = config[nameof(ISQLConnectionSettings.Password)]?.ToString();
            return data;
        }

        public static IList<MenuItem> ToMenuItems(this IDictionary<string, object> config)
        {
            IList<MenuItem> data = new List<MenuItem>();
            if (!(config?.Count > 0))
                return null;
            MenuItem menu;
            foreach (var row in config)
            {
                string codedMenu = row.Value?.ToString();
                string tag = null; string text = null;
                if (!(string.IsNullOrWhiteSpace(codedMenu)))
                {
                    text = codedMenu.Split(':')[0];
                    try { tag = codedMenu.Split(':')[1]; } catch { }
                    menu = new MenuItem(text, tag);
                    data.Add(menu);
                }
            }

            return data;
        }
      
        public static IList<MenuItem> ToMenuItems(this IDictionary<string, string> config)
        {
            IList<MenuItem> data = new List<MenuItem>();
            if (!(config?.Count > 0))
                return null;
            MenuItem menu;
            foreach (var row in config)
            {
                string codedMenu = row.Value?.ToString();
                string tag = null; string text = null;
                if (!(string.IsNullOrWhiteSpace(codedMenu)))
                {
                    text = codedMenu.Split(':')[0];
                    try { tag = codedMenu.Split(':')[1]; } catch { }
                    menu = new MenuItem(text, tag);
                    data.Add(menu);
                }
            }

            return data;
        }
    }
}
