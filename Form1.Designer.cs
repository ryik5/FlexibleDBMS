namespace AutoAnalyse
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtbResultShow = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.administratorMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.createLocalDBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromTextFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeModelsListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisDataMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getFIOMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.changeViewPanelviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.schemeLocalDBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnImage = new System.Windows.Forms.ToolStripDropDownButton();
            this.StatusLabellInfoDB = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtbQuery = new System.Windows.Forms.TextBox();
            this.lblQuery = new System.Windows.Forms.Label();
            this.getEnterpriseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtbResultShow
            // 
            this.txtbResultShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbResultShow.Location = new System.Drawing.Point(0, 2);
            this.txtbResultShow.Multiline = true;
            this.txtbResultShow.Name = "txtbResultShow";
            this.txtbResultShow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbResultShow.Size = new System.Drawing.Size(800, 396);
            this.txtbResultShow.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.administratorMenu,
            this.analysisDataMenu,
            this.dataMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // administratorMenu
            // 
            this.administratorMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createLocalDBMenuItem,
            this.importFromTextFileMenuItem,
            this.writeModelsListMenuItem});
            this.administratorMenu.Name = "administratorMenu";
            this.administratorMenu.Size = new System.Drawing.Size(92, 20);
            this.administratorMenu.Text = "Administrator";
            // 
            // createLocalDBMenuItem
            // 
            this.createLocalDBMenuItem.Name = "createLocalDBMenuItem";
            this.createLocalDBMenuItem.Size = new System.Drawing.Size(181, 22);
            this.createLocalDBMenuItem.Text = "Create localDB";
            // 
            // importFromTextFileMenuItem
            // 
            this.importFromTextFileMenuItem.Name = "importFromTextFileMenuItem";
            this.importFromTextFileMenuItem.Size = new System.Drawing.Size(181, 22);
            this.importFromTextFileMenuItem.Text = "Import from text file";
            // 
            // writeModelsListMenuItem
            // 
            this.writeModelsListMenuItem.Name = "writeModelsListMenuItem";
            this.writeModelsListMenuItem.Size = new System.Drawing.Size(181, 22);
            this.writeModelsListMenuItem.Text = "Write models List";
            // 
            // analysisDataMenu
            // 
            this.analysisDataMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analysisToolStripMenuItem,
            this.loadDataMenuItem,
            this.getFIOMenuItem,
            this.getEnterpriseMenuItem});
            this.analysisDataMenu.Name = "analysisDataMenu";
            this.analysisDataMenu.Size = new System.Drawing.Size(62, 20);
            this.analysisDataMenu.Text = "Analysis";
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            this.analysisToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.analysisToolStripMenuItem.Text = "Analysis";
            // 
            // loadDataMenuItem
            // 
            this.loadDataMenuItem.Name = "loadDataMenuItem";
            this.loadDataMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadDataMenuItem.Text = "Load data";
            // 
            // getFIOMenuItem
            // 
            this.getFIOMenuItem.Name = "getFIOMenuItem";
            this.getFIOMenuItem.Size = new System.Drawing.Size(180, 22);
            this.getFIOMenuItem.Text = "Get FIO";
            // 
            // dataMenu
            // 
            this.dataMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeViewPanelviewMenuItem,
            this.schemeLocalDBMenuItem});
            this.dataMenu.Name = "dataMenu";
            this.dataMenu.Size = new System.Drawing.Size(43, 20);
            this.dataMenu.Text = "Data";
            // 
            // changeViewPanelviewMenuItem
            // 
            this.changeViewPanelviewMenuItem.Name = "changeViewPanelviewMenuItem";
            this.changeViewPanelviewMenuItem.Size = new System.Drawing.Size(162, 22);
            this.changeViewPanelviewMenuItem.Text = "Show table";
            // 
            // schemeLocalDBMenuItem
            // 
            this.schemeLocalDBMenuItem.Name = "schemeLocalDBMenuItem";
            this.schemeLocalDBMenuItem.Size = new System.Drawing.Size(162, 22);
            this.schemeLocalDBMenuItem.Text = "Scheme local DB";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel1,
            this.btnImage,
            this.StatusLabellInfoDB});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel1
            // 
            this.StatusLabel1.Name = "StatusLabel1";
            this.StatusLabel1.Size = new System.Drawing.Size(668, 17);
            this.StatusLabel1.Spring = true;
            this.StatusLabel1.Text = "StatusLabel1";
            this.StatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnImage
            // 
            this.btnImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnImage.Image = ((System.Drawing.Image)(resources.GetObject("btnImage.Image")));
            this.btnImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImage.Name = "btnImage";
            this.btnImage.Size = new System.Drawing.Size(29, 20);
            this.btnImage.Text = "toolStripDropDownButton1";
            // 
            // StatusLabellInfoDB
            // 
            this.StatusLabellInfoDB.Name = "StatusLabellInfoDB";
            this.StatusLabellInfoDB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StatusLabellInfoDB.Size = new System.Drawing.Size(88, 17);
            this.StatusLabellInfoDB.Text = "StatusLabelInfo";
            this.StatusLabellInfoDB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.txtbResultShow);
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(803, 400);
            this.panel1.TabIndex = 5;
            // 
            // txtbQuery
            // 
            this.txtbQuery.Location = new System.Drawing.Point(502, 3);
            this.txtbQuery.Name = "txtbQuery";
            this.txtbQuery.Size = new System.Drawing.Size(296, 20);
            this.txtbQuery.TabIndex = 6;
            // 
            // lblQuery
            // 
            this.lblQuery.AutoSize = true;
            this.lblQuery.Location = new System.Drawing.Point(463, 8);
            this.lblQuery.Name = "lblQuery";
            this.lblQuery.Size = new System.Drawing.Size(35, 13);
            this.lblQuery.TabIndex = 7;
            this.lblQuery.Text = "Query";
            this.lblQuery.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // getEnterpriseToolStripMenuItem
            // 
            this.getEnterpriseMenuItem.Name = "getEnterpriseToolStripMenuItem";
            this.getEnterpriseMenuItem.Size = new System.Drawing.Size(180, 22);
            this.getEnterpriseMenuItem.Text = "Get Enterprise";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblQuery);
            this.Controls.Add(this.txtbQuery);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbResultShow;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem administratorMenu;
        private System.Windows.Forms.ToolStripMenuItem analysisDataMenu;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem writeModelsListMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataMenu;
        private System.Windows.Forms.ToolStripMenuItem changeViewPanelviewMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem importFromTextFileMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem createLocalDBMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDataMenuItem;
        private System.Windows.Forms.TextBox txtbQuery;
        private System.Windows.Forms.Label lblQuery;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabellInfoDB;
        private System.Windows.Forms.ToolStripDropDownButton btnImage;
        private System.Windows.Forms.ToolStripMenuItem schemeLocalDBMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getFIOMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getEnterpriseMenuItem;
    }
}

