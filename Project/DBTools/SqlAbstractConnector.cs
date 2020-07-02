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
        public delegate void Message<TextEventArgs>(object sender, TextEventArgs e);
        public virtual event Message<TextEventArgs> EvntInfoMessage;

       // public async Task<DataTable> GetDataTable(System.Threading.CancellationToken token, string query)
       // {
       //     DataTable dt = new DataTable();
       //     int timeout = 3600;

       //     var task = GetDataTable1(dt,token, query);
       //     if (await Task.WhenAny(task, Task.Delay(timeout, token)) == task)
       //     { await task; }
       //     return task.Result;
       // }

       //async Task<DataTable> GetDataTable1(DataTable dt,System.Threading.CancellationToken token, string query)
       // {
       //     await Task.Run(() => dt = GetTable(query), token);
       //     return dt;
       // }
    }
}
