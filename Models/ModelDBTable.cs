using System.Collections.Generic;

namespace FlexibleDBMS
{
    public interface IModelDBable<T>
    {
        string Name { get; set; }
        string Alias { get; set; }
        IList<T> Collection { get; set; }
    }

    /// <summary>
    /// Collection is Tables collection
    /// </summary>
    public class ModelDB : IModelDBable<ModelDBTable>
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
        public IList<ModelDBTable> Collection { get; set; }
    }

    /// <summary>
    /// Collection is columns collection
    /// </summary>
    public class ModelDBTable : IModelDBable<ModelDBColumn>
    {
        public string Name { get; set; }
        
        /// <summary>
        /// Alias of Name
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Columns
        /// </summary>
        public IList<ModelDBColumn> Collection { get; set; }
    }
    
    /// <summary>
    /// Collection is collection of Filters of the column
    /// </summary>
    public class ModelDBColumn : IModelDBable<ModelDBFilter>
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
        public IList<ModelDBFilter> Collection { get; set; }
    }

    /// <summary>
    /// Filter name
    /// </summary>
    public class ModelDBFilter
    {
        public string Name { get; set; }
       
        /// <summary>
        /// Alias of Name
        /// </summary>
        public string Alias { get; set; }
    }
}
