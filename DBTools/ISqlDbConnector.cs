using System.Data;

namespace FlexibleDBMS
{
    public interface ISqlDbConnector
    {
        DataTable GetTable(string query);
        ISQLConnectionSettings GetSettings();
    }
}
