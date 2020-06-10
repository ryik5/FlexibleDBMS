using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    [Serializable]
    public class ConfigFull<T> where T : ConfigAbstract, IComparable<ConfigAbstract>
    {
        public string Version { get; set; } = Application.ProductVersion;
        public DateTime LastModification { get; set; } = DateTime.Now;
        public ConfigType TypeConfigUnit { get; set; } = ConfigType.Application;

        IDictionary<string, T> config { get; set; }

        public ConfigFull() { config = new Dictionary<string, T>(); }

        public ConfigFull(ConfigFull<T> configNew)
        {
            Set(configNew);
        }

        public void Set(ConfigFull<T> configNew)
        {
            config = configNew.config;
            LastModification = configNew.LastModification;
            Version = configNew.Version;
            TypeConfigUnit = configNew.TypeConfigUnit;
        }

        public int Count() { return config?.Count ?? 0; }

        public IDictionary<string, T> Config { get { return config; } }

        public void Add(T newConfig)
        {
            if (newConfig == null || newConfig?.Name == null)
                return;

            if (config == null)
            { config = new Dictionary<string, T>(); }

            config[newConfig.Name] = newConfig;
        }

        public void Remove(string nameUnit)
        {
            if (GetUnit(nameUnit) != null)
                config.Remove(nameUnit);
        }

        public T GetUnit(string text)
        {
            config.TryGetValue(text, out T result);

            return result;
        }

        public ConfigFull<T> Get()
        {
            return this;
        }

        //Get whole keys in Dictionary
        public IList<string> GetUnitConfigNames()
        {
            IList<string> unitList = new List<string>(config.Keys.Distinct());
            IList<string> result = unitList.Except(CommonConst.UNIT_DEFAULT_LIST).ToList();

            return result;
        }
    }
}