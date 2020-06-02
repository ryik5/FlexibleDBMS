using System.Windows.Forms;

namespace FlexibleDBMS
{
    public static class MenuItemToToolStripMenuItemExtensions
    {
        //public static ToolStripMenuItem ToToolStripMenuItem(this MenuItem item, ToolStripMenuType modes)
        //{
        //    ToolStripMenuItem toolMenu =  item.ToQueryMenuToolStripMenuItem();                        

        //    return toolMenu;
        //}

        public static ToolStripMenuItem ToQueryMenuToolStripMenuItem(this MenuItem menuItem)
        {
            ToolStripMenuItem item = new ToolStripMenuItem()
            {
                Name = menuItem.Name,
                Text = menuItem.Text,
                Tag = menuItem.Tag,
                Size = new System.Drawing.Size(180, 22)
            };
            return item;
        }
    }
}
