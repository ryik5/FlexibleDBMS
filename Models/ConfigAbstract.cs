using System;
using System.Collections.Generic;

namespace FlexibleDBMS
{
    [Serializable]
    public abstract class ConfigAbstract : IComparable<ConfigAbstract>
    {
        public abstract string Name { get; set; }
        public abstract IDictionary<string, object> ConfigDictionary { get; set; }
        //Для возможности поиска дубляжного значения
        public override string ToString() => $"{Name}"; 

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ConfigAbstract df))
                return false;

            return ToString().Equals(df.ToString());
        }

        public override int GetHashCode()
        { return ToString().GetHashCode(); }
        
        //реализация для выполнения сортировки
        int IComparable<ConfigAbstract>.CompareTo(ConfigAbstract next)
        { return new ConfigComparerAbstract().Compare(this, next); }

        public string CompareTo(ConfigAbstract next)
        { return next.CompareTo(this); }
    }

    [Serializable]
    public class ConfigComparerAbstract : IComparer<ConfigAbstract>
    {
        public int Compare(ConfigAbstract x, ConfigAbstract y)
        { return CompareTwoConfig(x, y); }

        public int CompareTwoConfig(ConfigAbstract x, ConfigAbstract y)
        {
            string a = x.Name;
            string b = y.Name;

            return CompareTwoStrings.Compare(a, b);
        }
    }
}
