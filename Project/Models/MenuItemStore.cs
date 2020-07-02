using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public abstract class MenuAbstractStore
    {
        public abstract void Set(IList<ToolStripMenuItem> list);
        public abstract void Add(ToolStripMenuItem item);
        public abstract void Remove(string item);
        public abstract void Refresh();
        public abstract void Clear();
        public abstract ToolStripMenuItem GetItem(string text);
        public abstract IList<ToolStripMenuItem> GetAllItems();
    }

    public class MenuItemStore : MenuAbstractStore
    {
        readonly object locker = new object();

        IDictionary<string, ToolStripMenuItem> ItemDictionary { get; set; }
        public delegate void ItemAddedInCollection<BoolEventArgs>(object sender, BoolEventArgs e);
        public event ItemAddedInCollection<BoolEventArgs> EvntCollectionChanged;

        public MenuItemStore() { }

        public override void Add(ToolStripMenuItem item)
        {
            lock (locker)
            {
                if (ItemDictionary == null)
                { ItemDictionary = new Dictionary<string, ToolStripMenuItem>(); }

                ItemDictionary[item.Text] = item;

                EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
            }
        }

        public override void Set(IList<ToolStripMenuItem> menuList)
        {
            lock (locker)
            {
                ItemDictionary = new Dictionary<string, ToolStripMenuItem>();

                if (menuList?.Count > 0)
                {
                    foreach (var menu in menuList)
                    {
                        Add(menu);
                    }

                }
            }
            EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public override void Remove(string item)
        {
            lock (locker)
            {
                if (!(ItemDictionary?.Count > 0) || string.IsNullOrWhiteSpace(item))
                { return; }

                ToolStripMenuItem menu = GetItem(item);
                if (menu == null)
                { return; }

                ItemDictionary.Remove(item);

                EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
            }
        }

        public override void Refresh()
        {
            EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public override void Clear()
        {
            ItemDictionary = new Dictionary<string, ToolStripMenuItem>();

            EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public override ToolStripMenuItem GetItem(string text)
        {
            if (ItemDictionary == null || string.IsNullOrWhiteSpace(text))
            { return null; }

            bool existed = ItemDictionary.TryGetValue(text, out ToolStripMenuItem item);

            if (!existed)
            { return null; }

            return item;
        }

        public override IList<ToolStripMenuItem> GetAllItems()
        {
            if (ItemDictionary == null)
            { return null; }

            IList<ToolStripMenuItem> items = new List<ToolStripMenuItem>(ItemDictionary.Values?.OrderBy(x => x.Text)?.ToList());
            if (items?.Count > 0)
                return items;
            else
                return null;

        }
    }
}