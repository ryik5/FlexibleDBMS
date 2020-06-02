using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public class MenuFiltersMaker
    {
        IDictionary<string, string> dicTranslator;

        public MenuFiltersMaker(IDictionary<string, string> dicTranslator)
        { this.dicTranslator = dicTranslator; }

        public MenuFiltersMaker() { }

        public ToolStripSplitButton MakeDropDownToSplitButton(string menuTag, string menuText)
        {
            MenuItem menuItem = new MenuItem(menuText.Translate(dicTranslator), menuText);
            ToolStripSplitButton btnFilter1 = new ToolStripSplitButton();
            btnFilter1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilter1.ImageTransparentColor = Color.Magenta;
            btnFilter1.Name = menuItem.Name;
            btnFilter1.Tag = menuTag;
            btnFilter1.Size = new Size(73, 20);
            btnFilter1.Text = menuText;
            return btnFilter1;
        }

        public ToolStripMenuItem MakeFilterMenuItem(string menuName)
        {
            MenuItem menuItem = new MenuItem(menuName);
            ToolStripMenuItem subFilter = new ToolStripMenuItem();
            subFilter.Name = menuItem.Name;
            subFilter.Size = new Size(180, 22);
            subFilter.Text = menuItem.Text;
            return subFilter;
        }
    }
}