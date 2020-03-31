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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.administratorMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.createLocalDBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.importFromTextFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeModelsListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.changeViewPanelviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.addQueryExtraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.queriesStandartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.schemeLocalDBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.analysisToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getFIOMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getEnterpriseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.queriesExtraMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabelMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatuslabelBtnImage = new System.Windows.Forms.ToolStripDropDownButton();
            this.StatusLabelExtraInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtbBodyQuery = new System.Windows.Forms.TextBox();
            this.txtbNameQuery = new System.Windows.Forms.TextBox();
            this.removeQueryExtraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtbResultShow
            // 
            this.txtbResultShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbResultShow.Location = new System.Drawing.Point(0, 1);
            this.txtbResultShow.Multiline = true;
            this.txtbResultShow.Name = "txtbResultShow";
            this.txtbResultShow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbResultShow.Size = new System.Drawing.Size(798, 397);
            this.txtbResultShow.TabIndex = 1;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.administratorMenu,
            this.viewMenu,
            this.queryMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // administratorMenu
            // 
            this.administratorMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createLocalDBMenuItem,
            this.toolStripSeparator5,
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
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(178, 6);
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
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeViewPanelviewMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "View";
            // 
            // changeViewPanelviewMenuItem
            // 
            this.changeViewPanelviewMenuItem.Name = "changeViewPanelviewMenuItem";
            this.changeViewPanelviewMenuItem.Size = new System.Drawing.Size(132, 22);
            this.changeViewPanelviewMenuItem.Text = "Show table";
            // 
            // queryMenu
            // 
            this.queryMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addQueryExtraMenuItem,
            this.toolStripSeparator1,
            this.queriesStandartMenu,
            this.queriesExtraMenu,
            this.toolStripSeparator4,
            this.removeQueryExtraMenuItem});
            this.queryMenu.Name = "queryMenu";
            this.queryMenu.Size = new System.Drawing.Size(51, 20);
            this.queryMenu.Text = "Query";
            // 
            // addQueryExtraMenuItem
            // 
            this.addQueryExtraMenuItem.Name = "addQueryExtraMenuItem";
            this.addQueryExtraMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addQueryExtraMenuItem.Text = "Add";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // queriesStandartMenu
            // 
            this.queriesStandartMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.schemeLocalDBMenuItem,
            this.toolStripSeparator2,
            this.analysisToolStripMenuItem1,
            this.loadDataMenuItem,
            this.getFIOMenuItem,
            this.getEnterpriseMenuItem,
            this.toolStripSeparator3});
            this.queriesStandartMenu.Name = "queriesStandartMenu";
            this.queriesStandartMenu.Size = new System.Drawing.Size(180, 22);
            this.queriesStandartMenu.Text = "Standart queries";
            // 
            // schemeLocalDBMenuItem
            // 
            this.schemeLocalDBMenuItem.Name = "schemeLocalDBMenuItem";
            this.schemeLocalDBMenuItem.Size = new System.Drawing.Size(162, 22);
            this.schemeLocalDBMenuItem.Text = "Scheme local DB";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(159, 6);
            // 
            // analysisToolStripMenuItem1
            // 
            this.analysisToolStripMenuItem1.Name = "analysisToolStripMenuItem1";
            this.analysisToolStripMenuItem1.Size = new System.Drawing.Size(162, 22);
            this.analysisToolStripMenuItem1.Text = "Analysis";
            // 
            // loadDataMenuItem
            // 
            this.loadDataMenuItem.Name = "loadDataMenuItem";
            this.loadDataMenuItem.Size = new System.Drawing.Size(162, 22);
            this.loadDataMenuItem.Text = "Load data";
            // 
            // getFIOMenuItem
            // 
            this.getFIOMenuItem.Name = "getFIOMenuItem";
            this.getFIOMenuItem.Size = new System.Drawing.Size(162, 22);
            this.getFIOMenuItem.Text = "Get FIO";
            // 
            // getEnterpriseMenuItem
            // 
            this.getEnterpriseMenuItem.Name = "getEnterpriseMenuItem";
            this.getEnterpriseMenuItem.Size = new System.Drawing.Size(162, 22);
            this.getEnterpriseMenuItem.Text = "Get Enterprise";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(159, 6);
            // 
            // queriesExtraMenu
            // 
            this.queriesExtraMenu.Name = "queriesExtraMenu";
            this.queriesExtraMenu.Size = new System.Drawing.Size(180, 22);
            this.queriesExtraMenu.Text = "Extra queries";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(177, 6);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelMain,
            this.StatuslabelBtnImage,
            this.StatusLabelExtraInfo});
            this.statusStrip.Location = new System.Drawing.Point(0, 428);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip";
            // 
            // StatusLabelMain
            // 
            this.StatusLabelMain.Name = "StatusLabelMain";
            this.StatusLabelMain.Size = new System.Drawing.Size(642, 17);
            this.StatusLabelMain.Spring = true;
            this.StatusLabelMain.Text = "StatusLabelMain";
            this.StatusLabelMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatuslabelBtnImage
            // 
            this.StatuslabelBtnImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.StatuslabelBtnImage.Image = ((System.Drawing.Image)(resources.GetObject("StatuslabelBtnImage.Image")));
            this.StatuslabelBtnImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StatuslabelBtnImage.Name = "StatuslabelBtnImage";
            this.StatuslabelBtnImage.Size = new System.Drawing.Size(29, 20);
            this.StatuslabelBtnImage.Text = "toolStripDropDownButton1";
            // 
            // StatusLabelExtraInfo
            // 
            this.StatusLabelExtraInfo.Name = "StatusLabelExtraInfo";
            this.StatusLabelExtraInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StatusLabelExtraInfo.Size = new System.Drawing.Size(114, 17);
            this.StatusLabelExtraInfo.Text = "StatusLabelExtraInfo";
            this.StatusLabelExtraInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.panel1.Size = new System.Drawing.Size(803, 404);
            this.panel1.TabIndex = 5;
            // 
            // txtbBodyQuery
            // 
            this.txtbBodyQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbBodyQuery.Location = new System.Drawing.Point(502, 3);
            this.txtbBodyQuery.Name = "txtbBodyQuery";
            this.txtbBodyQuery.Size = new System.Drawing.Size(296, 20);
            this.txtbBodyQuery.TabIndex = 6;
            // 
            // txtbNameQuery
            // 
            this.txtbNameQuery.Location = new System.Drawing.Point(399, 3);
            this.txtbNameQuery.Name = "txtbNameQuery";
            this.txtbNameQuery.Size = new System.Drawing.Size(97, 20);
            this.txtbNameQuery.TabIndex = 7;
            // 
            // removeQueryExtraMenuItem
            // 
            this.removeQueryExtraMenuItem.Name = "removeQueryExtraMenuItem";
            this.removeQueryExtraMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeQueryExtraMenuItem.Text = "Remove";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtbNameQuery);
            this.Controls.Add(this.txtbBodyQuery);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbResultShow;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem administratorMenu;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem writeModelsListMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem changeViewPanelviewMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem importFromTextFileMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelMain;
        private System.Windows.Forms.ToolStripMenuItem createLocalDBMenuItem;
        private System.Windows.Forms.TextBox txtbBodyQuery;
        private System.Windows.Forms.Label lblQuery;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelExtraInfo;
        private System.Windows.Forms.ToolStripDropDownButton StatuslabelBtnImage;
        private System.Windows.Forms.ToolStripMenuItem queryMenu;
        private System.Windows.Forms.ToolStripMenuItem addQueryExtraMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queriesStandartMenu;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadDataMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getFIOMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getEnterpriseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queriesExtraMenu;
        private System.Windows.Forms.ToolStripMenuItem schemeLocalDBMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.TextBox txtbNameQuery;
        private System.Windows.Forms.ToolStripMenuItem removeQueryExtraMenuItem;
    }
}

