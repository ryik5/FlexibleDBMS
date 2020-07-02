namespace FlexibleDBMS
{
    partial class AdministratorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxSample = new System.Windows.Forms.ListBox();
            this.AdminStatusStrip = new System.Windows.Forms.StatusStrip();
            this.AdminStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.currentTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updatePlateRegionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectedDbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getNewConnectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fromTextFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromExcelFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxColumn = new System.Windows.Forms.ComboBox();
            this.listBoxColumn = new System.Windows.Forms.ListBox();
            this.labelColumns = new System.Windows.Forms.Label();
            this.labelRemoveColumns = new System.Windows.Forms.Label();
            this.labelSelectedColumn = new System.Windows.Forms.Label();
            this.labelеTable = new System.Windows.Forms.Label();
            this.comboBoxTable = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AdminStatusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxSample
            // 
            this.listBoxSample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxSample.FormattingEnabled = true;
            this.listBoxSample.Location = new System.Drawing.Point(0, 3);
            this.listBoxSample.Name = "listBoxSample";
            this.listBoxSample.Size = new System.Drawing.Size(522, 368);
            this.listBoxSample.TabIndex = 0;
            // 
            // AdminStatusStrip
            // 
            this.AdminStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AdminStatusLabel});
            this.AdminStatusStrip.Location = new System.Drawing.Point(0, 428);
            this.AdminStatusStrip.Name = "AdminStatusStrip";
            this.AdminStatusStrip.Size = new System.Drawing.Size(800, 22);
            this.AdminStatusStrip.TabIndex = 1;
            this.AdminStatusStrip.Text = "AdminStatusStrip";
            // 
            // AdminStatusLabel
            // 
            this.AdminStatusLabel.Name = "AdminStatusLabel";
            this.AdminStatusLabel.Size = new System.Drawing.Size(103, 17);
            this.AdminStatusLabel.Text = "AdminStatusLabel";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentTableToolStripMenuItem,
            this.getNewConnectionMenuItem,
            this.importMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menu";
            // 
            // currentTableToolStripMenuItem
            // 
            this.currentTableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updatePlateRegionToolStripMenuItem,
            this.clearSelectedDbToolStripMenuItem,
            this.deleteSelectedColumnsToolStripMenuItem});
            this.currentTableToolStripMenuItem.Name = "currentTableToolStripMenuItem";
            this.currentTableToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.currentTableToolStripMenuItem.Text = "CurrentTable";
            // 
            // updatePlateRegionToolStripMenuItem
            // 
            this.updatePlateRegionToolStripMenuItem.Name = "updatePlateRegionToolStripMenuItem";
            this.updatePlateRegionToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.updatePlateRegionToolStripMenuItem.Text = "UpdatePlateRegion";
            // 
            // clearSelectedDbToolStripMenuItem
            // 
            this.clearSelectedDbToolStripMenuItem.Name = "clearSelectedDbToolStripMenuItem";
            this.clearSelectedDbToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.clearSelectedDbToolStripMenuItem.Text = "ClearTable";
            // 
            // deleteSelectedColumnsToolStripMenuItem
            // 
            this.deleteSelectedColumnsToolStripMenuItem.Name = "deleteSelectedColumnsToolStripMenuItem";
            this.deleteSelectedColumnsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.deleteSelectedColumnsToolStripMenuItem.Text = "DeleteSelectedColumns";
            // 
            // getNewConnectionMenuItem
            // 
            this.getNewConnectionMenuItem.Name = "getNewConnectionMenuItem";
            this.getNewConnectionMenuItem.Size = new System.Drawing.Size(123, 20);
            this.getNewConnectionMenuItem.Text = "GetNewConnection";
            // 
            // importMenu
            // 
            this.importMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromTextFileToolStripMenuItem,
            this.fromExcelFileToolStripMenuItem});
            this.importMenu.Name = "importMenu";
            this.importMenu.Size = new System.Drawing.Size(55, 20);
            this.importMenu.Text = "Import";
            // 
            // fromTextFileToolStripMenuItem
            // 
            this.fromTextFileToolStripMenuItem.Name = "fromTextFileToolStripMenuItem";
            this.fromTextFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromTextFileToolStripMenuItem.Text = "FromTextFile";
            // 
            // fromExcelFileToolStripMenuItem
            // 
            this.fromExcelFileToolStripMenuItem.Name = "fromExcelFileToolStripMenuItem";
            this.fromExcelFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromExcelFileToolStripMenuItem.Text = "FromExcelFile";
            // 
            // comboBoxColumn
            // 
            this.comboBoxColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxColumn.FormattingEnabled = true;
            this.comboBoxColumn.Location = new System.Drawing.Point(533, 101);
            this.comboBoxColumn.Name = "comboBoxColumn";
            this.comboBoxColumn.Size = new System.Drawing.Size(255, 21);
            this.comboBoxColumn.TabIndex = 4;
            // 
            // listBoxColumn
            // 
            this.listBoxColumn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxColumn.FormattingEnabled = true;
            this.listBoxColumn.Location = new System.Drawing.Point(533, 161);
            this.listBoxColumn.Name = "listBoxColumn";
            this.listBoxColumn.ScrollAlwaysVisible = true;
            this.listBoxColumn.Size = new System.Drawing.Size(255, 264);
            this.listBoxColumn.Sorted = true;
            this.listBoxColumn.TabIndex = 5;
            // 
            // labelColumns
            // 
            this.labelColumns.AutoSize = true;
            this.labelColumns.Location = new System.Drawing.Point(533, 82);
            this.labelColumns.Name = "labelColumns";
            this.labelColumns.Size = new System.Drawing.Size(97, 13);
            this.labelColumns.TabIndex = 6;
            this.labelColumns.Text = "Список столбцов:";
            // 
            // labelRemoveColumns
            // 
            this.labelRemoveColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRemoveColumns.AutoSize = true;
            this.labelRemoveColumns.Location = new System.Drawing.Point(531, 136);
            this.labelRemoveColumns.Name = "labelRemoveColumns";
            this.labelRemoveColumns.Size = new System.Drawing.Size(143, 13);
            this.labelRemoveColumns.TabIndex = 7;
            this.labelRemoveColumns.Text = "Удалить столбцы из базы:";
            // 
            // labelSelectedColumn
            // 
            this.labelSelectedColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSelectedColumn.AutoSize = true;
            this.labelSelectedColumn.Location = new System.Drawing.Point(7, 35);
            this.labelSelectedColumn.Name = "labelSelectedColumn";
            this.labelSelectedColumn.Size = new System.Drawing.Size(0, 13);
            this.labelSelectedColumn.TabIndex = 8;
            // 
            // labelеTable
            // 
            this.labelеTable.AutoSize = true;
            this.labelеTable.Location = new System.Drawing.Point(533, 35);
            this.labelеTable.Name = "labelеTable";
            this.labelеTable.Size = new System.Drawing.Size(85, 13);
            this.labelеTable.TabIndex = 10;
            this.labelеTable.Text = "Список таблиц:";
            // 
            // comboBoxTable
            // 
            this.comboBoxTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTable.FormattingEnabled = true;
            this.comboBoxTable.Location = new System.Drawing.Point(533, 54);
            this.comboBoxTable.Name = "comboBoxTable";
            this.comboBoxTable.Size = new System.Drawing.Size(255, 21);
            this.comboBoxTable.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.listBoxSample);
            this.panel1.Location = new System.Drawing.Point(3, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(522, 373);
            this.panel1.TabIndex = 11;
            // 
            // AdministratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelеTable);
            this.Controls.Add(this.comboBoxTable);
            this.Controls.Add(this.labelSelectedColumn);
            this.Controls.Add(this.labelRemoveColumns);
            this.Controls.Add(this.labelColumns);
            this.Controls.Add(this.listBoxColumn);
            this.Controls.Add(this.comboBoxColumn);
            this.Controls.Add(this.AdminStatusStrip);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AdministratorForm";
            this.Text = "AdministratorForm";
            this.Load += new System.EventHandler(this.AdministratorForm_Load);
            this.AdminStatusStrip.ResumeLayout(false);
            this.AdminStatusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip AdminStatusStrip;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem currentTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updatePlateRegionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectedDbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importMenu;
        private System.Windows.Forms.ToolStripMenuItem getNewConnectionMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel AdminStatusLabel;
        private System.Windows.Forms.ComboBox comboBoxColumn;
        private System.Windows.Forms.ListBox listBoxColumn;
        private System.Windows.Forms.Label labelColumns;
        private System.Windows.Forms.Label labelRemoveColumns;
        private System.Windows.Forms.ListBox listBoxSample;
        private System.Windows.Forms.Label labelSelectedColumn;
        private System.Windows.Forms.Label labelеTable;
        private System.Windows.Forms.ComboBox comboBoxTable;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedColumnsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromTextFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromExcelFileToolStripMenuItem;
    }
}