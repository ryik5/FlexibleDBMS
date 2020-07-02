using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public static class ToolStripExtensions
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

            foreach (var v in item.DropDownItems)
            {
                if (v is ToolStripMenuItem)
                {
                    ToolStripMenuItem m = v as ToolStripMenuItem;

                    list.Add(m);
                }
            }

            return list;
        }

        public static IList<ToolStripMenuItem> ToToolStripMenuItemsList(this IList<RegistryEntity> list, ToolStripMenuType modes)
        {
            IList<ToolStripMenuItem> toolMenuList = new List<ToolStripMenuItem>();

            MenuItem menuItem;
            string text, tag;

            if (list?.Count > 0)
            {
                foreach (var r in list)
                {
                    switch (modes)
                    {
                        case ToolStripMenuType.ExtraQuery:
                            {
                                if (r.ValueKind == Microsoft.Win32.RegistryValueKind.String)
                                {
                                    text = r?.Value?.ToString()?.Trim().Split(':')[0]?.Trim();
                                    tag = r?.Value?.ToString()?.Trim().Split(':')[1]?.Trim();

                                    menuItem = new MenuItem(text, tag);
                                    if (text?.Length > 0 && tag?.Length > 0)
                                    {
                                        ToolStripMenuItem toolMenu = menuItem.ToQueryMenuToolStripMenuItem();
                                        toolMenuList.Add(toolMenu);
                                    }
                                }
                                break;
                            }
                        case ToolStripMenuType.RecentConnection:
                            {
                                text = r?.Value?.ToString()?.Trim();
                                tag = r?.Key?.ToString()?.Trim();

                                menuItem = new MenuItem(text, tag);
                                if (text?.Length > 0 && tag?.Length > 0)
                                {
                                    ToolStripMenuItem toolMenu = menuItem.ToFilterToolStripMenuItem();
                                    toolMenuList.Add(toolMenu);
                                }

                                break;
                            }
                    }
                }
            }

            return toolMenuList;
        }

        public static IList<ToolStripMenuItem> ToToolStripMenuItemsList(this IList<string> list, ToolStripMenuType modes)
        {
            IList<ToolStripMenuItem> toolMenuList = new List<ToolStripMenuItem>();
            ToolStripMenuItem toolMenu = null;
            MenuItem menuItem;

            if (list?.Count > 0)
            {
                foreach (var text in list)
                {
                    if (text?.Length > 0)
                    {
                        menuItem = new MenuItem(text, text);
                        toolMenu = menuItem.ToToolStripMenuItem(modes);

                        toolMenuList.Add(toolMenu);
                    }
                }
            }

            return toolMenuList;
        }

        public static IList<ToolStripMenuItem> ToToolStripMenuItemList(this IDictionary<string, MenuItem> items, ToolStripMenuType modes)
        {
            IList<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
            ToolStripMenuItem toolMenu = null;
            foreach (var item in items)
            {
                toolMenu = item.Value.ToToolStripMenuItem(modes);

                list.Add(toolMenu);
            }

            return list;
        }

        public static IList<ToolStripMenuItem> ToToolStripMenuItemList(this IList< MenuItem> items, ToolStripMenuType modes)
        {
            IList<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
            ToolStripMenuItem toolMenu = null;
            foreach (var item in items)
            {
                toolMenu = item.ToToolStripMenuItem(modes);

                list.Add(toolMenu);
            }

            return list;
        }


        public static ToolStripMenuItem ToToolStripMenuItem(this MenuItem item, ToolStripMenuType modes)
        {
            ToolStripMenuItem toolMenu = null;

            switch (modes)
            {
                case ToolStripMenuType.ExtraQuery:
                case ToolStripMenuType.StandartQuery:
                    {
                        toolMenu = item.ToQueryMenuToolStripMenuItem();
                        break;
                    }
                case ToolStripMenuType.RecentConnection:
                    {

                        toolMenu = item.ToFilterToolStripMenuItem();

                        break;
                    }
                default:
                    break;
            }

            return toolMenu;
        }


        /// <summary>
        /// Convert ToolStripDropDownItem to IDictionary<itemName,itemText: itemTag>
        /// </summary>
        /// <param name="item">ToolStripDropDownItem, is used itemName, itemText and itemTag</param>
        /// <returns></returns>
        public static IDictionary<string, string> AsDictionary(this ToolStripDropDownItem item, int maxAmountElementsSubMenu = 1000)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>(maxAmountElementsSubMenu);
            IList<string> lines = new List<string>();
            MenuItem menuItem;
            string text, tag;

            foreach (var v in item.DropDownItems)
            {
                if (v is ToolStripMenuItem)
                {
                    ToolStripMenuItem m = v as ToolStripMenuItem;
                    text = m.Text?.Replace(":", "")?.Replace("  ", " ")?.Trim();
                    tag = m.Tag?.ToString()?.Replace(":", "")?.Replace("  ", " ")?.Trim();
                    if (text?.Length > 0 && tag?.Length > 0)
                    {
                        if (lines.Where(x => x.ToLower().Equals(tag)).Count() == 0)
                        {
                            if (maxAmountElementsSubMenu < 1)
                            {
                                continue;
                            }
                            menuItem = new MenuItem(text, tag);
                            lines.Add(tag.ToLower());
                            dic.Add(menuItem.Name, $"{text}: {tag}");
                            maxAmountElementsSubMenu--;
                        }
                    }
                }
            }

            return dic;
        }
      
        /// <summary>
        /// Convert ToolStripDropDownItem to IDictionary<itemName,itemText: itemTag>
        /// </summary>
        /// <param name="item">ToolStripDropDownItem, is used itemName, itemText and itemTag</param>
        /// <returns></returns>
        public static IDictionary<string, object> AsObjectDictionary(this ToolStripDropDownItem item, int maxAmountElementsSubMenu = 1000)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>(maxAmountElementsSubMenu);
            IList<string> lines = new List<string>();
            MenuItem menuItem;
            string text, tag;

            foreach (var v in item.DropDownItems)
            {
                if (v is ToolStripMenuItem)
                {
                    ToolStripMenuItem m = v as ToolStripMenuItem;
                    text = m.Text?.Replace(":", "")?.Replace("  ", " ")?.Trim();
                    tag = m.Tag?.ToString()?.Replace(":", "")?.Replace("  ", " ")?.Trim();
                    if (text?.Length > 0 && tag?.Length > 0)
                    {
                        if (lines.Where(x => x.ToLower().Equals(tag)).Count() == 0)
                        {
                            if (maxAmountElementsSubMenu < 1)
                            {
                                continue;
                            }
                            menuItem = new MenuItem(text, tag);
                            lines.Add(tag.ToLower());
                            dic[menuItem.Name] = $"{text}: {tag}";

                            maxAmountElementsSubMenu--;
                        }
                    }
                }
            }

            return dic;
        }

     
          public static ToolStripMenuItem ToQueryMenuToolStripMenuItem(this MenuItem menuItem)
        {
            ToolStripMenuItem item = new ToolStripMenuItem()
            {
                Name = menuItem.Name,
                Text = menuItem.Text,
                Tag = menuItem.Tag,
                Checked = true,
                CheckOnClick = true,
                CheckState = CheckState.Unchecked,
                // DoubleClickEnabled = true,
                Size = new System.Drawing.Size(180, 22),
            };
            return item;
        }
        
        /// <summary>
        /// Filter Menu Item
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        public static ToolStripMenuItem ToFilterToolStripMenuItem(this MenuItem menuItem)
        {
            ToolStripMenuItem item = new ToolStripMenuItem()
            {
               Text  = menuItem.Text,
                Tag = menuItem.Tag,
                Size = new System.Drawing.Size(180, 22),
            };
            return item;
        }


    }
}