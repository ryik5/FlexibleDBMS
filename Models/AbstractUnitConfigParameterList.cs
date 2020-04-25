using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    [Serializable]
    public abstract class AbstractUnitConfigParameterList
    {
        public string Name { get; set; } = "new unit List";
        public string Desciption { get; set; } = "New Unit List";
        public virtual string Version { get; set; } = Application.ProductVersion;
        public virtual DateTime LastModification { get; set; } = DateTime.Now;
        public virtual TypeConfig TypeConfigUnit { get; set; } = TypeConfig.None;
        public virtual int Count { get; }
        public abstract void Set(IDictionary<string, AbstractConfigParameter> parameterList);
        public abstract void Add(AbstractConfigParameter config);
        public abstract IDictionary<string, AbstractConfigParameter> Get { get; }
        public abstract AbstractConfigParameter GetParameter(string text);

    }
}
