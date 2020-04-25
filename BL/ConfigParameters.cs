using System;
using System.Collections.Generic;

namespace FlexibleDBMS
{
    [Serializable]
    public class ConfigParameter : AbstractConfigParameter
    {
        object lockChanging = new object();
        public delegate void ConfigChanged(object sender, BoolEventArgs args);
        public event ConfigChanged EvntConfigChanged;

        IDictionary<string, string> config;

        public override int Count
        {
            get
            {
                if (config == null)
                {
                    config = new Dictionary<string, string>();
                    return 0;
                }
                else { return config.Count; }
            }
        }

        public ConfigParameter() { config = new Dictionary<string, string>(); }

        public ConfigParameter(AbstractConfigParameter newConfig)
        { config = newConfig?.Get; }

        public ConfigParameter(IDictionary<string, string> parameterList) { Set(parameterList); }

        public override void Set(IDictionary<string, string> parameterList)
        {
            lock (lockChanging)
            {
                if (parameterList != null && parameterList.Count > 0)
                    config = parameterList;
            }
        }

        public override void Add(string parameter, string value)
        {
            if (parameter == null || value == null)
                return;

            lock (lockChanging)
            {
                if (config == null)
                {
                    config = new Dictionary<string, string>();
                }

                bool exist = config.TryGetValue(parameter, out string oldConfig);
                if (exist)
                {
                    config[parameter] = value;
                }
                else
                {
                    config.Add(parameter, value);
                }
            }
        }

        public override string GetParameter(string text)
        {
            lock (lockChanging)
            {
                config.TryGetValue(text, out string result);

                return result;
            }
        }

        public override IDictionary<string, string> Get { get { return config; } }
    }
}
