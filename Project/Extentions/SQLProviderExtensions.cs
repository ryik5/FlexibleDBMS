using System;
using System.Collections.Generic;

namespace FlexibleDBMS
{

    [Serializable]
    public static class SQLProviderExtensions
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
