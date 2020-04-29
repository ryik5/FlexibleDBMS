using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlexibleDBMS
{

    [Serializable]
    public abstract class AbstractConfig
    {
        public abstract string Name { get; set; }
        public abstract IDictionary<string, object> ConfigDictionary { get; set; }
    }

    [Serializable]
    public class Config : AbstractConfig
    {
        object lockChanging = new object();
        IDictionary<string, object> config { get; set; } = new Dictionary<string, object>();
        public override string Name { get; set; } = "";
        public virtual string Version { get; set; } = Application.ProductVersion;
        public DateTime LastModification { get; set; } = DateTime.Now;
        public TypeConfig TypeConfigUnit { get; set; } = TypeConfig.None;

        public override IDictionary<string, object> ConfigDictionary { get { return config; } set { config = value; } }
        public void Add(AbstractConfig newConfig)
        {
            if (newConfig == null)
                return;

            lock (lockChanging)
            {
                if (config == null)
                { config = new Dictionary<string, object>(); }

                config[newConfig.Name] = newConfig;
            }
        }
    }

    [Serializable]
    public class ConfigFullNew<T> where T : AbstractConfig
    {
        public string Version { get; set; } = Application.ProductVersion;
        public DateTime LastModification { get; set; } = DateTime.Now;
        public TypeConfig TypeConfigUnit { get; set; } = TypeConfig.Application;

        IDictionary<string, T> config { get; set; }

        public ConfigFullNew() { config = new Dictionary<string, T>(); }
        
        public ConfigFullNew(ConfigFullNew<T> configFullNew) 
        { 
            this.config = configFullNew.config;
            this.LastModification = configFullNew.LastModification;
            this.Version = configFullNew.Version;
            this.TypeConfigUnit = configFullNew.TypeConfigUnit;
        }

        public int Count() { return config?.Count ?? 0; }

        public IDictionary<string, T> Config { get { return config; } }

        public void Add(T newConfig)
        {
            if (newConfig == null|| newConfig?.Name==null)
                return;

                 if (config == null)
                { config = new Dictionary<string, T>(); }

                config[newConfig.Name] = newConfig;
        }

        public T Get(string text)
        {
            config.TryGetValue(text, out T result);

            return result;
        }

        public ConfigFullNew<T> Get()
        {
            return this;
        }
    
        //Get whole keys in Dictionary
        public IList<string> GetAllNameConfigs()
        {
            IList<string> result = new List<string>(config.Keys);
            return result;
        }
    }

    public class ConfigDictionaryTo
    {
        public ConfigDictionaryTo() { }

        public SQLConnectionData ToSQLConnectionData(IDictionary<string, object> config)
        {
            SQLConnectionData data = new SQLConnectionData();
            data.Name = config[nameof(ISQLConnectionSettings.Name)]?.ToString();
            data.Database = config[nameof(ISQLConnectionSettings.Database)]?.ToString();
            data.Table = config[nameof(ISQLConnectionSettings.Table)]?.ToString();
            data.ProviderName = config[nameof(ISQLConnectionSettings.ProviderName)]?.ToString().GetSQLProvider();
            data.Host = config[nameof(ISQLConnectionSettings.Host)]?.ToString();
            data.Port = int.TryParse(config[nameof(ISQLConnectionSettings.Port)]?.ToString(), out int port) ? port : 0;
            data.Username = config[nameof(ISQLConnectionSettings.Username)]?.ToString();
            data.Password = config[nameof(ISQLConnectionSettings.Password)]?.ToString();
            return data;
        }

        public ISQLConnectionSettings ToISQLConnectionSettings(IDictionary<string, object> config)
        {
            ISQLConnectionSettings data = new SQLConnectionSettings();
            if (!(config?.Count > 0))
                return data;

            data.Name = config[nameof(ISQLConnectionSettings.Name)]?.ToString();
            data.Database = config[nameof(ISQLConnectionSettings.Database)]?.ToString();
            data.Table = config[nameof(ISQLConnectionSettings.Table)]?.ToString();
            data.ProviderName = config[nameof(ISQLConnectionSettings.ProviderName)]?.ToString().GetSQLProvider();
            data.Host = config[nameof(ISQLConnectionSettings.Host)]?.ToString();
            data.Port = int.TryParse(config[nameof(ISQLConnectionSettings.Port)]?.ToString(), out int port) ? port : 0;
            data.Username = config[nameof(ISQLConnectionSettings.Username)]?.ToString();
            data.Password = config[nameof(ISQLConnectionSettings.Password)]?.ToString();
            return data;
        }
        public IList<MenuItem> ToMenuItems(IDictionary<string, object> config)
        {
            IList<MenuItem> data = new List<MenuItem>();
            if (!(config?.Count > 0))
                return null;
            MenuItem menu;
            foreach (var row in config)
            {
                string codedMenu = row.Value?.ToString();
                if (!(string.IsNullOrWhiteSpace(codedMenu)))
                {
                    string text = codedMenu.Split(':')[0];
                    string tag = codedMenu.Split(':')[1];
                    menu = new MenuItem(text, tag);
                    data.Add(menu);
                }
            }

            return data;
        }
    }
}
