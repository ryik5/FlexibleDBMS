using System.Collections.Generic;

namespace FlexibleDBMS
{

    /// <summary>
    /// Collection is Tables collection
    /// </summary>
    public class DBModel : IModelEntityDB<DBTableModel>
    {
        public string Name { get; set; }

        /// <summary>
        /// Alias of Name
        /// </summary>
        public string Alias { get; set; }
        public string FilePath { get; set; }
        public string SqlConnectionString { get; set; }

        /// <summary>
        /// Tables
        /// </summary>
        public IList<DBTableModel> ColumnCollection { get; set; }
    }
}
