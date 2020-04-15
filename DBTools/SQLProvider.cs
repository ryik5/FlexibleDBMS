using System.Collections.Generic;

namespace AutoAnalysis
{
    public enum SQLProvider
    {
        None,
        MS_SQL,
        My_SQL,
        SQLite
    }

    public static class SQLProviderManager
    {
        public static IList<string> GetSQLProvider()
        {
            IList<string> list = new List<string>();
            foreach (var provider in EnumExtensions.GetEnumValueCollection<SQLProvider>())
            {
                list.Add(provider.ToString());
            }

            return list;
        }
    }
}
