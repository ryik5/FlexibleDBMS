using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FlexibleDBMS
{

    [Serializable]
    public abstract class AbstractConfigParameter
    {
        public string Name { get; set; } = "new parameter";
        public string Desciption { get; set; } = "new parameter";
        public virtual string Version { get; set; } = Application.ProductVersion;
        public virtual DateTime LastModification { get; set; } = DateTime.Now;
        public virtual TypeConfig TypeConfigUnit { get; set; } = TypeConfig.None;
        public virtual int Count { get; } = 0;

        public abstract void Set(IDictionary<string, string> parameterList);

        public abstract void Add(string parameter, string value);
        public abstract IDictionary<string, string> Get { get; }
        public abstract string GetParameter(string text);
    }
}
