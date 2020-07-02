using System;

namespace FlexibleDBMS
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
    }

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

    [Serializable]
    public class SQLConnectionStore
    {
        ISQLConnectionSettings connection;
        public delegate void ConfigChanged(object sender, BoolEventArgs args);
        public event ConfigChanged EvntConfigChanged;

        public SQLConnectionStore() { }

        public ISQLConnectionSettings Get()
        {
            return connection;
        }

        public void Set(ISQLConnectionSettings settings)
        {
            connection =new SQLConnectionSettings( settings);
            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }
    }

    [Serializable]
    public class SQLConnectionSettings : ISQLConnectionSettings
    {
        public string Host { get; set; } = "local";
        public int? Port { get; set; } = 0;
        public string Database { get; set; } = "main.db";
        public string Table { get; set; } = "main";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = $"local - main.db";
        public SQLProvider? ProviderName { get; set; } = SQLProvider.SQLite;

        public SQLConnectionSettings() { }

        public SQLConnectionSettings(ISQLConnectionSettings settings)
        { Set(settings); }

        public void Set(ISQLConnectionSettings settings)
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
