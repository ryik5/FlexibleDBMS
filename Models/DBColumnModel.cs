using System.Collections.Generic;

namespace FlexibleDBMS
{

    /// <summary>
    /// Collection is collection of Filters of the column
    /// </summary>
    public class DBColumnModel : IModelEntityDB<DBFilterModel>
    {
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Alias of Name
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Type of data the column
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// collection of Filters of the column
        /// </summary>
        public IList<DBFilterModel> ColumnCollection { get; set; }
    }
}
