using System.Data;

namespace FlexibleDBMS
{
    public class DataTableStore
    {
        readonly object locker = new object();
       public string Errors { get; set; }
        DataTable Data { get; set; }
        public delegate void DataTableCollection<BoolEventArgs>(object sender, BoolEventArgs e);
        public event DataTableCollection<BoolEventArgs> EvntDataTableChanged;

        public DataTableStore() { }

        public void Set(DataTable dataTable)
        {
            lock (locker)
            {
                if (dataTable?.Rows?.Count > 0)
                {
                    Data = dataTable.Copy();
                    Errors = null;
                }
                else
                {
                    Data = null;
                }
            }
            EvntDataTableChanged?.Invoke(this, new BoolEventArgs(true));
        }
        public void Set(DataTableStore dataTable)
        {
            lock (locker)
            {
                if (dataTable?.Data?.Rows?.Count > 0)
                {
                    Data = dataTable.Data.Copy();
                    Errors = null;
                }
                else
                {
                    Data = new DataTable();
                    Errors = dataTable?.Errors;
                }
            }
            EvntDataTableChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public void Refresh()
        {
            EvntDataTableChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public void Clear()
        {
            Data = new DataTable();

            EvntDataTableChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public DataTable GetDataTable()
        {
            if (Data == null)
            { return null; }

            return Data.Copy();
        }
    }
}
