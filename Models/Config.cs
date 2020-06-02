using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    [Serializable]
    public class Config : ConfigAbstract, IComparable<ConfigAbstract>
    {
        object lockChanging = new object();
        IDictionary<string, object> config { get; set; } = new Dictionary<string, object>();
        public override string Name { get; set; } = "";
        public virtual string Version { get; set; } = Application.ProductVersion;
        public DateTime LastModification { get; set; } = DateTime.Now;
        public TypeConfig TypeConfigUnit { get; set; } = TypeConfig.None;

        public override IDictionary<string, object> ConfigDictionary { get { return config; } set { config = value; } }
        public void Add(ConfigAbstract newConfig)
        {
            if (newConfig == null)
                return;

            lock (lockChanging)
            {
                if (config == null)
                { config = new Dictionary<string, object>(); }

                //if (Get(newConfig?.Name) != null)
                //    return;

                config[newConfig.Name] = newConfig;
            }
        }
       
        public void Add(ISQLConnectionSettings newConfig)
        {
            if (newConfig?.Name == null)
                return;

            lock (lockChanging)
            {
                if (config == null)
                { config = new Dictionary<string, object>(); }

                config[newConfig.Name] = newConfig.DoObjectPropertiesAsObjectDictionary();
            }
        }

        public object Get(string text)
        {
            config.TryGetValue(text, out object result);
            return result;
        }

        public int Compare(ConfigAbstract x, ConfigAbstract y)
        {
            string a = x.Name;
            string b = y.Name;

            return CompareTwoStrings.Compare(a, b);
        }
    }
}
