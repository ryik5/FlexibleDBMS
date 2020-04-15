namespace AutoAnalysis
{
    public interface ISQLConnectionSettings
    {
        string Host { get; set; }
        int? Port { get; set; }
        string Database { get; set; }
        string Table { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Name { get; set; }
        SQLProvider? ProviderName { get; set; }
        bool IsGlobal { get; }
    }

    public class SQLConnectionSettings : ISQLConnectionSettings
    {
        public string Host { get; set; } = "local";
        public int? Port { get; set; } = 0;
        public string Database { get; set; } = "main.db";
        public string Table { get; set; } = "CarAndOwner";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = "local - Main.db";
        public SQLProvider? ProviderName { get; set; } = SQLProvider.SQLite;
        public bool IsGlobal => false;

        public SQLConnectionSettings() { }

        public SQLConnectionSettings(ISQLConnectionSettings settings)
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
    }
}
