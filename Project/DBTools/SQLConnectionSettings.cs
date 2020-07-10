using System;

namespace FlexibleDBMS
{
    [Serializable]
    public class SQLConnectionSettings : ISQLConnectionSettings
    {
        public string Host { get; set; } = "local";
        public int? Port { get; set; } = 0;
        public string Database { get; set; } = "main.db";
        public string Table { get; set; } = "MainData";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get { return $"{Host} - {Database}"; } set { Name = value; } }
        public SQLProvider? ProviderName { get; set; } = SQLProvider.SQLite;

        public SQLConnectionSettings() { }

        public SQLConnectionSettings(ISQLConnectionSettings settings)
        { Set(settings); }

        public void Set(ISQLConnectionSettings settings)
        {
            if (settings != null)
            {
                ProviderName = settings?.ProviderName;
                Host = settings?.Host;
                Port = settings?.Port;
                Username = settings?.Username;
                Password = settings?.Password;
                Database = settings?.Database;
                Table = settings?.Table;
            }
        }

        public SQLConnectionSettings Get()
        {
            return new SQLConnectionSettings()
            {
                Name = this?.Name,
                ProviderName = this?.ProviderName,
                Host = this?.Host,
                Port = this?.Port,
                Username = this?.Username,
                Password = this?.Password,
                Database = this?.Database,
                Table = this?.Table
            };
        }

        public override string ToString()
        {
            return this.DoObjectPropertiesAsStringDictionary().AsString();
        }
    }
}
