using System.Collections.Generic;

namespace FlexibleDBMS
{    public interface IModels
    {
        IDictionary<int, IModel> list { get; set; }

        IList<IModel> ToModelList();

        IList<string> ToList();
    }
}
