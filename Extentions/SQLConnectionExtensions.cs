using System.Collections.Generic;

namespace FlexibleDBMS
{
    public static class SQLConnectionExtensions
    {
        public static SQLProvider GetSQLProvider(this string provider)
        {
            SQLProvider deffinedSQLProvider = SQLProvider.MS_SQL;
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

        public static ISQLConnectionSettings ToSQLConnectionSettings(this SQLConnectionData data)
        {
            ISQLConnectionSettings newSettings = new SQLConnectionSettings();
            newSettings.Host = data.Host;
            newSettings.Database = data.Database;
            newSettings.Name = data.Name;
            newSettings.Port = data.Port;
            newSettings.Table = data.Table;
            newSettings.Username = data.Username;
            newSettings.Password = data.Password;
            newSettings.ProviderName = data.ProviderName;

            return newSettings;
        }

        public static SQLConnectionData ToSQLConnectionData(this ISQLConnectionSettings data)
        {
            SQLConnectionData newSettings = new SQLConnectionData();
            newSettings.Host = data.Host;
            newSettings.Database = data.Database;
            newSettings.Name = data.Name;
            newSettings.Port = data.Port;
            newSettings.Table = data.Table;
            newSettings.Username = data.Username;
            newSettings.Password = data.Password;
            newSettings.ProviderName = data.ProviderName;

            return newSettings;
        }

        public static ISQLConnectionSettings ToSQLConnectionSettings(this IList<RegistryEntity> entities)
        {
            ISQLConnectionSettings connectionSettings = new SQLConnectionSettings();

            if (!(entities?.Count > 0))
            {
                return connectionSettings;
            }

            foreach (var entity in entities)
            {
                string key = entity?.Key;
                switch (key)
                {
                    case "Name":
                        {
                            connectionSettings.Name = entity?.Value.ToString();
                            break;
                        }
                    case "ProviderName":
                        {
                            connectionSettings.ProviderName = entity?.Value.ToString().GetSQLProvider();
                            break;
                        }
                    case "Host":
                        {
                            connectionSettings.Host = entity?.Value.ToString();
                            break;
                        }
                    case "Port":
                        {
                            connectionSettings.Port = int.TryParse(entity?.Value.ToString(), out int port) ? port : 0;
                            break;
                        }
                    case "Username":
                        {
                            connectionSettings.Username = entity?.Value.ToString();
                            break;
                        }
                    case "Password":
                        {
                            connectionSettings.Password = entity?.Value.ToString();
                            break;
                        }
                    case "Database":
                        {
                            connectionSettings.Database = entity?.Value.ToString();
                            break;
                        }
                    case "Table":
                        {
                            connectionSettings.Table = entity?.Value.ToString();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return connectionSettings;
        }
    }
}