using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public class ModelCommonStore : IModels
    {
        public IDictionary<int, IModel> list { get; set; }

        public ModelCommonStore()
        {
            list = new Dictionary<int, IModel>();
        }

        public override string ToString()
        {
            string result = string.Empty;

            foreach (var c in list)
            {
                result += $"{c.Value.Name} - {c.Value.Alias} | ";
            }

            return result.TrimEnd(' ').TrimEnd('|');
        }

        public IList<IModel> ToModelList()
        {
            IList<IModel> list = new List<IModel>();
            foreach (var c in this.list.OrderBy(x => x.Key))
            {
                list.Add(c.Value);
            }
            return list;
        }


        public IList<string> ToList()
        {
            IList<string> result = new List<string>();
            if (list?.Count > 0)
            {
                foreach (var model in list)
                {
                    result.Add($"{model.Value.Name} ({model.Value.Alias})");
                }
            }
            return result?.OrderBy(x => x)?.ToList();
        }
    }
}