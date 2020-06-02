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
}
