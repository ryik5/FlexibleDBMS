using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public static class ToolStripDropDownItemExtensions
    {
        public static IList<ToolStripMenuItem> ToToolStripMenuItemList(this IList<string> items)
        {
            IList<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
            if (items?.Count > 0)
            {
                foreach (var v in items)
                {

                    ToolStripMenuItem m = (new MenuItem(v)).ToQueryMenuToolStripMenuItem();

                    list.Add(m);
                }
            }
            return list;
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

        /// <summary>
        /// Convert ToolStripDropDownItem to IDictionary<itemName,itemText: itemTag>
        /// </summary>
        /// <param name="menues">ToolStripDropDownItem, is used itemName, itemText and itemTag</param>
        /// <returns></returns>
        public static IDictionary<string, object> AsObjectDictionary(this IList<ToolStripMenuItem> menues, int maxAmountElementsSubMenu = 1000)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>(maxAmountElementsSubMenu);
            IList<string> lines = new List<string>();
            MenuItem menuItem;
            string text, tag;

            foreach (var v in menues)
            {
                text = v.Text?.Replace(":", "")?.Replace("  ", " ")?.Trim();
                tag = v.Tag?.ToString()?.Replace(":", "")?.Replace("  ", " ")?.Trim();
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

            return dic;
        }
    }
}