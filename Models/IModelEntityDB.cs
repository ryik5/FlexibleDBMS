using System.Collections.Generic;

namespace FlexibleDBMS
{
    public interface IModelEntityDB<T>
    {
        string Name { get; set; }
        string Alias { get; set; }
        IList<T> Collection { get; set; }
    }
}
