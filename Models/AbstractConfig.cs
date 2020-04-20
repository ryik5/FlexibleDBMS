using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AutoAnalysis
{

    [Serializable]
    public abstract class AbstractConfig
    {
        //public delegate void ConfigChanged(object sender, BoolEventArgs args);
        //public event ConfigChanged EvntConfigChanged;
        public string Name { get; set; }
        public string Desciption { get; set; }
        public virtual string Version { get; set; } = Application.ProductVersion;
        public virtual DateTime LastModification { get; set; } = DateTime.Now;
        public virtual TypeConfig TypeConfigUnit { get; set; }
        public virtual int ParameterAmount { get; }

        public virtual void Set(IDictionary<string, object> parameterList) { }

        public virtual void Add(AbstractConfig parameter) { }
        public virtual void Add(string parameter, object value) { }
        public virtual object GetParameter(string text) { return new object(); }
        public virtual IDictionary<string, object> Get() { return new Dictionary<string, object>(); }
    }
}
