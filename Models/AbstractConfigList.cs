using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    [Serializable]
    public abstract class AbstractConfigList
    {
        public string Name { get; set; } = "new Config List";
        public string Desciption { get; set; } = "New Config List";
        public virtual string Version { get; set; } = Application.ProductVersion;
        public virtual DateTime LastModification { get; set; } = DateTime.Now;
        public virtual TypeConfig TypeConfigUnit { get; set; } = TypeConfig.None;

        public virtual int Count { get; }
        public abstract void Set(IDictionary<string, AbstractUnitConfigParameterList> parameterList);
        public abstract void Add(AbstractUnitConfigParameterList config);
        public abstract IDictionary<string, AbstractUnitConfigParameterList> Get { get; }
        public abstract AbstractUnitConfigParameterList GetParameter(string text);

    }
}
