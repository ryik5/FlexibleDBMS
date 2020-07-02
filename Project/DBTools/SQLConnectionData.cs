using System;

namespace FlexibleDBMS
{
    [Serializable]
    public class SQLConnectionData : ISQLConnectionSettings
    {
        public string Host { get; set; } = "local";
        public int? Port { get; set; } = 0;
        public string Database { get; set; } = "main.db";
        public string Table { get; set; } = "main";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = $"local - main.db";
        public SQLProvider? ProviderName { get; set; } = SQLProvider.SQLite;

        public SQLConnectionData() { }

        public SQLConnectionData(SQLConnectionData settings)
        {
            Name = settings?.Name;
            ProviderName = settings?.ProviderName;
            Host = settings?.Host;
            Port = settings?.Port;
            Username = settings?.Username;
            Password = settings?.Password;
            Database = settings?.Database;
            Table = settings?.Table;
        }

        public SQLConnectionData Get()
        {
            return new SQLConnectionData()
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
    }

}
