using System;
using System.Collections.Generic;

namespace FlexibleDBMS
{

    [Serializable]
    public class ConfigUnitParameterList : AbstractUnitConfigParameterList
    {
        object lockChanging = new object();

        IDictionary<string, AbstractConfigParameter> config;

        public delegate void ConfigChanged(object sender, BoolEventArgs args);
        public  event ConfigChanged EvntConfigChanged;

        public ConfigUnitParameterList()
        { config = new Dictionary<string, AbstractConfigParameter>(); }

        public ConfigUnitParameterList(AbstractUnitConfigParameterList newconfig)
        { config = newconfig?.Get; }

        public override int Count
        {
            get
            {
                if (config == null)
                {
                    config = new Dictionary<string, AbstractConfigParameter>();
                    return 0;
                }
                else { return config.Count; }
            }
        }

        public override IDictionary<string, AbstractConfigParameter> Get { get { return config; } }

        public override void Add(AbstractConfigParameter parameter)
        {
            if (parameter == null || string.IsNullOrWhiteSpace(parameter?.Name))
                return;

            lock (lockChanging)
            {
                if (config == null)
                {
                    config = new Dictionary<string, AbstractConfigParameter>();
                }

                config[parameter.Name] = parameter;
            }

            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public override void Set(IDictionary<string, AbstractConfigParameter> parameterList)
        {
            lock (lockChanging)
            {
                if (parameterList != null && parameterList.Count > 0)
                    config = parameterList;
            }
        }

        public override AbstractConfigParameter GetParameter(string text)
        {
            config.TryGetValue(text,out AbstractConfigParameter result);
            
            return result;
        }
    }
}
