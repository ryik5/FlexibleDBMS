using System;
using System.Collections.Generic;

namespace FlexibleDBMS
{
    [Serializable]
    public class ConfigList : AbstractConfigList
    {
        object lockChanging = new object();

        IDictionary<string, AbstractUnitConfigParameterList> config;

        public delegate void ConfigChanged(object sender, BoolEventArgs args);
        public event ConfigChanged EvntConfigChanged;

        public ConfigList()
        { config = new Dictionary<string, AbstractUnitConfigParameterList>(); }

        public ConfigList(AbstractConfigList newConfig) 
        {
            config = newConfig?.Get; 
        }

        public override int Count
        {
            get
            {
                if (config == null)
                {
                    config = new Dictionary<string, AbstractUnitConfigParameterList>();
                    return 0;
                }
                else { return config.Count; }
            }
        }

        public override IDictionary<string, AbstractUnitConfigParameterList> Get { get { return config; } }

        public override void Add(AbstractUnitConfigParameterList newConfig)
        {
            if (newConfig == null || string.IsNullOrWhiteSpace(newConfig?.Name) || newConfig?.Count == 0)
                return;

            lock (lockChanging)
            {
                if (config == null)
                {
                    config = new Dictionary<string, AbstractUnitConfigParameterList>();
                }
                
                config[newConfig.Name] = newConfig;
            }

            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public override void Set(IDictionary<string, AbstractUnitConfigParameterList> parameterList)
        {
            throw new NotImplementedException();
        }

        public override AbstractUnitConfigParameterList GetParameter(string text)
        {
            config.TryGetValue(text,out AbstractUnitConfigParameterList result);
            return result;
        }
    }
}
