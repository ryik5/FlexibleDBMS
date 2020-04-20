using System;
using System.Collections.Generic;

namespace AutoAnalysis
{
    [Serializable]
    public class ConfigParameter : AbstractConfig
    {
        public delegate void ConfigChanged(object sender, BoolEventArgs args);
        public event ConfigChanged EvntConfigChanged;
        object lockChanging = new object();

        IDictionary<string, object> ConfigParameterList;
        public override int ParameterAmount
        {
            get
            {
                if (ConfigParameterList == null)
                {
                    ConfigParameterList = new Dictionary<string, object>();
                    return 0;
                }
                else { return ConfigParameterList.Count; }
            }
        }

        public ConfigParameter() { }
        public ConfigParameter(ConfigParameter parameter) { Add(parameter.Name, parameter); }
        public ConfigParameter(IDictionary<string, object> parameterList) { Set(parameterList); }

        public override void Set(IDictionary<string, object> parameterList)
        {
            lock (lockChanging)
            {
                ConfigParameterList = parameterList;
            }

            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public override void Add(AbstractConfig parameter)
        {
            lock (lockChanging)
            {
                if (ConfigParameterList == null)
                {
                    ConfigParameterList = new Dictionary<string, object>();
                }

                bool exist = ConfigParameterList.TryGetValue(parameter.Name, out object oldConfig);
                if (exist)
                {
                    ConfigParameterList[parameter.Name] = parameter;
                }
                else
                {
                    ConfigParameterList.Add(parameter.Name, parameter);
                }
            }

            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }
        public override void Add(string parameter, object value)
        {
            lock (lockChanging)
            {
                if (ConfigParameterList == null)
                {
                    ConfigParameterList = new Dictionary<string, object>();
                }

                bool exist = ConfigParameterList.TryGetValue(parameter, out object oldConfig);
                if (exist)
                {
                    ConfigParameterList[parameter] = value;
                }
                else
                {
                    ConfigParameterList.Add(parameter, value);
                }
            }

            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public override object GetParameter(string text)
        {
            lock (lockChanging)
            {
                bool exist = ConfigParameterList.TryGetValue(text, out object result);

                if (!exist)
                { return null; }
                else
                { return result; }
            }
        }

        public override IDictionary<string, object> Get()
        {
            return ConfigParameterList;
        }
    }
}
