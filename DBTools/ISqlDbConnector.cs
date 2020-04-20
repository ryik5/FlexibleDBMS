using System.Data;

namespace AutoAnalysis
{
    public interface ISqlDbConnector
    {
        DataTable GetTable(string query);
    }
}
