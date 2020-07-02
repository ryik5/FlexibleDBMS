using System;
using System.Collections.Generic;

namespace FlexibleDBMS
{
    [Serializable]
    public static class SQLConnectionExtensions
    {
        public static SQLProvider GetSQLProvider(this string provider)
        {
            SQLProvider deffinedSQLProvider;
            switch (provider)
            {
                case "MS_SQL":
                    deffinedSQLProvider = SQLProvider.MS_SQL;
                    break;
                case "My_SQL":
                    deffinedSQLProvider = SQLProvider.My_SQL;
                    break;
                case "SQLite":
                    deffinedSQLProvider = SQLProvider.SQLite;
                    break;
                case "None":
                default:
                    deffinedSQLProvider = SQLProvider.None;
                    break;
            }

            return deffinedSQLProvider;
        }

        public static IList<string> GetSQLProvider()
        {
            IList<string> list = new List<string>();
            foreach (var provider in EnumExtensions.GetEnumValueCollection<SQLProvider>())
            {
                list.Add(provider.ToString());
            }

            return list;
        }
        
        public static string AsString(this ISQLConnectionSettings settings)
        {
            return settings.DoObjectPropertiesAsStringDictionary().AsString();
        }
    }
}