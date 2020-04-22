using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AutoAnalysis
{
    public static class ModelsExtensions
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
                                        ToolStripMenuItem toolMenu = menuItem.ToExtraMenuToolStripMenuItem();
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

        /// <summary>
        /// Filter Menu Items
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<ToolStripMenuItem> ToFilterToolStripMenuItemsList(this IList<string> list)
        {
            IList<ToolStripMenuItem> filterMenuList = new List<ToolStripMenuItem>();
            MenuItem menuItem;

            if (list?.Count > 0)
            {
                foreach (var r in list)
                {
                    if (r?.Length > 0)
                    {
                        menuItem = new MenuItem(r, "");
                        ToolStripMenuItem toolMenu = menuItem.ToFilterToolStripMenuItem();
                        filterMenuList.Add(toolMenu);
                    }
                }
            }
            return filterMenuList;
        }


        /// <summary>
        /// Convert ToolStripDropDownItem to IDictionary<itemName,itemText: itemTag>
        /// </summary>
        /// <param name="item">ToolStripDropDownItem, is used itemName, itemText and itemTag</param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(this ToolStripDropDownItem item, int maxAmountElementsSubMenu = 40)
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

        public static IDictionary<string, string> GetPropertyValues(this object obj, int maxAmountClassElements = 1000)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>(maxAmountClassElements);

            if (obj == null) return null;

            Type t = obj.GetType();

            PropertyInfo[] props = t.GetProperties();

            if (props?.Length > 0)
            {
                foreach (var prop in props)
                {
                    if (prop.GetIndexParameters().Length == 0)
                    { dic.Add(prop?.Name, prop?.GetValue(obj)?.ToString()); }
                    else
                    { dic.Add(prop?.Name, prop?.PropertyType?.Name); }
                }
            }

            return dic;
        }


        public static string AsString(this IDictionary<string, string> dic)
        {
            string text = string.Empty;
            if (dic?.Count > 0)
            {
                foreach (var s in dic)
                {
                    text += $"{s.Key}:\t{s.Value}\r\n";
                }
            }

            return text;
        }


        public static ISQLConnectionSettings ToSQLConnectionSettings(this IList<RegistryEntity> entities)
        {
            ISQLConnectionSettings connectionSettings = new SQLConnectionSettings();

            //if entities is empty or null
            if (entities==null|| entities?.Count == 0)
            {
                return connectionSettings;
            }

            foreach (var entity in entities)
            {
                string key = entity?.Key;
                switch (key)
                {
                    case "Name":
                        {
                            connectionSettings.Name = entity?.Value.ToString();
                            break;
                        }
                    case "ProviderName":
                        {
                            connectionSettings.ProviderName = entity?.Value.ToString().GetSQLProvider();
                            break;
                        }
                    case "Host":
                        {
                            connectionSettings.Host = entity?.Value.ToString();
                            break;
                        }
                    case "Port":
                        {
                            connectionSettings.Port = int.TryParse(entity?.Value.ToString(), out int port) ? port : 0;
                            break;
                        }
                    case "Username":
                        {
                            connectionSettings.Username = entity?.Value.ToString();
                            break;
                        }
                    case "Password":
                        {
                            connectionSettings.Password = entity?.Value.ToString();
                            break;
                        }
                    case "Database":
                        {
                            connectionSettings.Database = entity?.Value.ToString();
                            break;
                        }
                    case "Table":
                        {
                            connectionSettings.Table = entity?.Value.ToString();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            
            return connectionSettings;
        }

        public static ToolStripMenuItem ToExtraMenuToolStripMenuItem(this MenuItem menuItem)
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
                Name = menuItem.Name,
                Text = menuItem.Text,
                Size = new System.Drawing.Size(180, 22),
            };
            return item;
        }


    }
}