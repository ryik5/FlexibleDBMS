using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FlexibleDBMS
{

    public class ModelCommonStringStore
    {
        public delegate void ItemAddedInCollection(object sender, BoolEventArgs e);
        public event ItemAddedInCollection EvntCollectionChanged;

        readonly object locker = new object();

        public IDictionary<string, IModel> ItemDictionary { get; set; }

        public ModelCommonStringStore()
        {
            ItemDictionary = new Dictionary<string, IModel>();
        }

        public ModelCommonStringStore(IDictionary<string, IModel> newStore)
        {
            Set(newStore);
        }

        public void Set(IDictionary<string, IModel> newStore)
        {
            lock (locker)
            {
                ItemDictionary = new Dictionary<string, IModel>();
                foreach (var m in newStore)
                {
                    ItemDictionary[m.Key] = m.Value;
                }
                EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
            }
        }

        public void Set(ModelCommonStringStore newStore)
        {
            lock (locker)
            {
                if (newStore?.ToModelList()?.Count > 0)
                {
                    ItemDictionary = new Dictionary<string, IModel>();
                    foreach (var m in newStore.ToModelList())
                    {
                        ItemDictionary[m.Name] = m;
                    }
                    EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
                }
            }
        }

        public void Reset()
        {
            lock (locker)
            {
                ItemDictionary = new Dictionary<string, IModel>();
                EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
            }
        }

        public void Remove(string item)
        {
            lock (locker)
            {
                if (!(ItemDictionary?.Count > 0) || string.IsNullOrWhiteSpace(item))
                { return; }

                IModel menu = GetItem(item);
                if (menu == null)
                { return; }

                ItemDictionary.Remove(item);
                EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
            }
        }

        public IModel GetItem(string text)
        {
            if (ItemDictionary == null || string.IsNullOrWhiteSpace(text))
            { return null; }

            bool existed = ItemDictionary.TryGetValue(text, out IModel item);

            if (!existed)
            { return null; }

            return item;
        }

        public IList<ToolStripMenuItem> GetToolStipMenuItemList()
        {
            IList<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
            foreach (var c in ItemDictionary.OrderBy(x => x.Key))
            {
                ToolStripMenuItem menu = new ToolStripMenuItem() { Text = c.Value.Name, Tag = c.Value.Alias };
                list.Add(menu);
            }

            return list;
        }

        public void Add(IModel model)
        {
            lock (locker)
            {
                if (ItemDictionary == null)
                { ItemDictionary = new Dictionary<string, IModel>(); }

                ItemDictionary[model.Name] = model;
                EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
            }
        }

        public override string ToString()
        {
            string result = string.Empty;

            foreach (var c in ItemDictionary)
            {
                result += $"{c.Value.Name} - {c.Value.Alias} | ";
            }
            result = result.TrimEnd(' ').TrimEnd('|');

            return result;
        }

        public IList<IModel> ToModelList()
        {
            IList<IModel> list = new List<IModel>();
            foreach (var c in ItemDictionary.OrderBy(x => x.Key))
            {
                list.Add(c.Value);
            }
            return list;
        }

        public IList<string> ToList()
        {
            IList<string> result = new List<string>();
            if (ItemDictionary?.Count > 0)
            {
                foreach (var model in ItemDictionary.OrderBy(x => x.Key))
                {
                    result.Add($"{model.Value.Name}"); // ({model.Value.Alias})
                }
            }
            return result;
        }
    }

}
