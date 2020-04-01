using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AutoAnalyse
{
  public static  class ModelsConvertor
    {
        public static IList<MenuItem> ToMenuItemsList(this IList<ToolStripItem> items)
        {
            IList<MenuItem> list = new List<MenuItem>();

            foreach (var m in items)
            {
                list.Add(new MenuItem(m.Text, m.Tag.ToString()));
            }

            return list;
        }

        public static IList<ToolStripItem> ToToolStripItemsList(this ToolStripDropDownItem item)
        {
            IList<ToolStripItem> list = new List<ToolStripItem>();

            foreach (ToolStripMenuItem m in item.DropDownItems)
            {
                list.Add(m);
            }

            return list;
        }
        
        /// <summary>
        /// Convert ToolStripDropDownItem to IDictionary<itemName,itemText: itemTag>
        /// </summary>
        /// <param name="item">ToolStripDropDownItem, is used itemName, itemText and itemTag</param>
        /// <returns></returns>
        public static IDictionary<string,string> ToDictionary(this ToolStripDropDownItem item, int limitElementsMenu)
        {
            IDictionary<string, string> dic = new Dictionary<string,string>(limitElementsMenu);
            IList<string> queries = new List<string>();
            MenuItem menuItem;
            string name, query;

            foreach (ToolStripMenuItem m in item.DropDownItems)
            {
                name = m.Text?.Replace(":", "")?.Replace("  ", " ")?.Trim();
                query = m.Tag?.ToString()?.Replace(":", "")?.Replace("  ", " ")?.Trim();
                if (name?.Length > 0 && query?.Length > 0)
                {
                    if (queries.Where(x => x.Equals(query)).Count() == 0)
                    {
                        if (limitElementsMenu < 1)
                        {
                            continue;
                        }
                        menuItem = new MenuItem(name, query);
                        queries.Add(query);
                        dic.Add(menuItem.Name, $"{name}: {query}");
                        limitElementsMenu--;
                    }
                }
            }

            return dic;
        }

        public static ToolStripMenuItem ToToolStripMenuItem(this MenuItem menuItem)
        {
            ToolStripMenuItem item = new ToolStripMenuItem()
            {
                Name = menuItem.Name,
                Text = menuItem.NameQuery,
                Tag = menuItem.BodyQuery,
                Checked = true,
                CheckOnClick = true,
                CheckState = CheckState.Unchecked,
                // DoubleClickEnabled = true,
                Size = new System.Drawing.Size(180, 22),
            };
            return item;
        }
        
        public static IList<ToolStripMenuItem> ToToolStripMenuItemsList(this IList<RegistryEntity> list)
        {
            IList<ToolStripMenuItem> toolMenuList = new List<ToolStripMenuItem>();

            MenuItem menuItem;
            string name, query;
   
                if (list?.Count > 0)
                {
                    foreach (var r in list)
                    {
                    if (r.Type == Microsoft.Win32.RegistryValueKind.String)
                    {
                        name = r?.Value?.ToString()?.Trim().Split(':')[0]?.Trim();
                        query = r?.Value?.ToString()?.Trim().Split(':')[1]?.Trim();

                        menuItem = new MenuItem(name, query);
                        if (name?.Length > 0 && query?.Length > 0)
                        {
                            ToolStripMenuItem toolMenu = menuItem.ToToolStripMenuItem();
                            toolMenuList.Add(toolMenu);
                        }
                    }
                }
            }

            return toolMenuList;
        }
    }
}
