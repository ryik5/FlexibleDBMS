using System.Data;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
    public abstract class SqlAbstractConnector
    {
        public abstract void SetConnection(ISQLConnectionSettings settings);
        public abstract DataTable GetTable(string query, int timeout = 3600);
        public abstract ISQLConnectionSettings GetConnection();
        public abstract void DoQuery(string query, bool isCommit = true);
        public  delegate void Message<TextEventArgs>(object sender, TextEventArgs e);
        public virtual event Message<TextEventArgs> EvntInfoMessage;

        public async Task<DataTable> GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            await Task.Run(() => dt = GetTable(query));

            return dt;
        }
    }
}
