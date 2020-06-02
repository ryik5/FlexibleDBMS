using System.Collections.Generic;

namespace FlexibleDBMS
{

    /// <summary>
    /// Collection is columns collection
    /// </summary>
    public class DBTableModel : IModelEntityDB<DBColumnModel>
    {
        public string Name { get; set; }
        
        /// <summary>
        /// Alias of Name
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Columns
        /// </summary>
        public IList<DBColumnModel> ColumnCollection { get; set; }
    }
}
