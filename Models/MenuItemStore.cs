using System.Collections.Generic;
using System.Linq;

namespace FlexibleDBMS
{

    public interface IMenuStore
    {
        void Add(MenuItem item);
        void Remove(MenuItem item);
        MenuItem GetMenuItem(string text);
    }

    public class MenuItemStore : IMenuStore
    {
        IDictionary<string, MenuItem> MenuItemDictionary { get; set; }
        public delegate void ItemAddedInCollection(object sender, BoolEventArgs e);
        public event ItemAddedInCollection EvntCollectionChanged;

        public MenuItemStore() { }

        public void Add(MenuItem item)
        {
            if (MenuItemDictionary == null)
            { MenuItemDictionary = new Dictionary<string, MenuItem>(); }

            MenuItemDictionary[item.Text] = item;
            EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public void Remove(MenuItem item)
        {
            if (!(MenuItemDictionary?.Count > 0) || string.IsNullOrWhiteSpace(item.Text))
            { return; }

            if (GetMenuItem(item.Text) == null)
            { return; }

            MenuItemDictionary.Remove(item.Text);
            EvntCollectionChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public MenuItem GetMenuItem(string text)
        {
            if (MenuItemDictionary == null || string.IsNullOrWhiteSpace(text))
            { return null; }

            bool existed = MenuItemDictionary.TryGetValue(text, out MenuItem item);

            if (!existed)
            { return null; }

            return item;
        }
        public IList<MenuItem> GetAllMenuItems()
        {
            if (MenuItemDictionary == null )
            { return null; }

            IList<MenuItem> items = MenuItemDictionary.Values?.ToList();

            if (!(items?.Count>0))
            { return null; }

            return items;
        }        
    }
}
