namespace AutoAnalysis
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
            this.selectNewDBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.importFromTextFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeModelsListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.changeViewPanelviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allColumnsInTableQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queryMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.addQueryExtraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.queriesStandartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.schemeLocalDBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.analysisToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.getFIOMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getEnterpriseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.loadDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadData1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadData2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queriesExtraMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.removeQueryExtraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusMessenges = new System.Windows.Forms.StatusStrip();
            this.StatusApp = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusInfoMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.SplitImage1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.StatusLabelExtraInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusFilters = new System.Windows.Forms.StatusStrip();
            this.StatusInfoFilter = new System.Windows.Forms.ToolStripStatusLabel();
            this.SplitImage2 = new System.Windows.Forms.ToolStripSplitButton();
            this.btnFilter1 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFilter2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnFilter3 = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFilter4 = new System.Windows.Forms.ToolStripDropDownButton();
            this.txtbBodyQuery = new System.Windows.Forms.TextBox();
            this.txtbNameQuery = new System.Windows.Forms.TextBox();
            this.menuStrip.SuspendLayout();
            this.statusMessenges.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusFilters.SuspendLayout();
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
            this.txtbResultShow.Size = new System.Drawing.Size(798, 349);
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
            this.selectNewDBMenuItem,
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
            this.createLocalDBMenuItem.Text = "Create local DB";
            // 
            // selectNewDBMenuItem
            // 
            this.selectNewDBMenuItem.Name = "selectNewDBMenuItem";
            this.selectNewDBMenuItem.Size = new System.Drawing.Size(181, 22);
            this.selectNewDBMenuItem.Text = "Select new DB";
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
            this.changeViewPanelviewMenuItem,
            this.updateFiltersMenuItem,
            this.allColumnsInTableQueryMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "View";
            // 
            // changeViewPanelviewMenuItem
            // 
            this.changeViewPanelviewMenuItem.Name = "changeViewPanelviewMenuItem";
            this.changeViewPanelviewMenuItem.Size = new System.Drawing.Size(201, 22);
            this.changeViewPanelviewMenuItem.Text = "Change view";
            // 
            // updateFiltersMenuItem
            // 
            this.updateFiltersMenuItem.Name = "updateFiltersMenuItem";
            this.updateFiltersMenuItem.Size = new System.Drawing.Size(201, 22);
            this.updateFiltersMenuItem.Text = "Update filters";
            // 
            // allColumnsInTableQueryMenuItem
            // 
            this.allColumnsInTableQueryMenuItem.Name = "allColumnsInTableQueryMenuItem";
            this.allColumnsInTableQueryMenuItem.Size = new System.Drawing.Size(201, 22);
            this.allColumnsInTableQueryMenuItem.Text = "All Columns in the table";
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
            this.addQueryExtraMenuItem.Size = new System.Drawing.Size(159, 22);
            this.addQueryExtraMenuItem.Text = "Add";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(156, 6);
            // 
            // queriesStandartMenu
            // 
            this.queriesStandartMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.schemeLocalDBMenuItem,
            this.toolStripSeparator2,
            this.analysisToolStripMenuItem1,
            this.getFIOMenuItem,
            this.getEnterpriseMenuItem,
            this.toolStripSeparator3,
            this.loadDataMenuItem,
            this.loadData1ToolStripMenuItem,
            this.loadData2ToolStripMenuItem});
            this.queriesStandartMenu.Name = "queriesStandartMenu";
            this.queriesStandartMenu.Size = new System.Drawing.Size(159, 22);
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
            // loadDataMenuItem
            // 
            this.loadDataMenuItem.Name = "loadDataMenuItem";
            this.loadDataMenuItem.Size = new System.Drawing.Size(162, 22);
            this.loadDataMenuItem.Text = "Load data";
            // 
            // loadData1ToolStripMenuItem
            // 
            this.loadData1ToolStripMenuItem.Name = "loadData1ToolStripMenuItem";
            this.loadData1ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.loadData1ToolStripMenuItem.Text = "Load data1";
            // 
            // loadData2ToolStripMenuItem
            // 
            this.loadData2ToolStripMenuItem.Name = "loadData2ToolStripMenuItem";
            this.loadData2ToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.loadData2ToolStripMenuItem.Text = "Load data 2";
            // 
            // queriesExtraMenu
            // 
            this.queriesExtraMenu.Name = "queriesExtraMenu";
            this.queriesExtraMenu.Size = new System.Drawing.Size(159, 22);
            this.queriesExtraMenu.Text = "Extra queries";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(156, 6);
            // 
            // removeQueryExtraMenuItem
            // 
            this.removeQueryExtraMenuItem.Name = "removeQueryExtraMenuItem";
            this.removeQueryExtraMenuItem.Size = new System.Drawing.Size(159, 22);
            this.removeQueryExtraMenuItem.Text = "Remove";
            // 
            // statusMessenges
            // 
            this.statusMessenges.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusApp,
            this.StatusInfoMain,
            this.SplitImage1,
            this.StatusLabelExtraInfo});
            this.statusMessenges.Location = new System.Drawing.Point(0, 428);
            this.statusMessenges.Name = "statusMessenges";
            this.statusMessenges.Size = new System.Drawing.Size(800, 22);
            this.statusMessenges.TabIndex = 4;
            this.statusMessenges.Text = "statusStrip";
            // 
            // StatusApp
            // 
            this.StatusApp.Name = "StatusApp";
            this.StatusApp.Size = new System.Drawing.Size(61, 17);
            this.StatusApp.Text = "StatusApp";
            // 
            // StatusInfoMain
            // 
            this.StatusInfoMain.Name = "StatusInfoMain";
            this.StatusInfoMain.Size = new System.Drawing.Size(581, 17);
            this.StatusInfoMain.Spring = true;
            this.StatusInfoMain.Text = "StatusLabelMain";
            this.StatusInfoMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SplitImage1
            // 
            this.SplitImage1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SplitImage1.Image = ((System.Drawing.Image)(resources.GetObject("SplitImage1.Image")));
            this.SplitImage1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SplitImage1.Name = "SplitImage1";
            this.SplitImage1.Size = new System.Drawing.Size(29, 20);
            this.SplitImage1.Text = "toolStripDropDownButton1";
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
            this.panel1.Size = new System.Drawing.Size(803, 375);
            this.panel1.TabIndex = 5;
            // 
            // statusFilters
            // 
            this.statusFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusInfoFilter,
            this.SplitImage2,
            this.btnFilter1,
            this.btnFilter2,
            this.btnFilter3,
            this.btnFilter4});
            this.statusFilters.Location = new System.Drawing.Point(0, 406);
            this.statusFilters.Name = "statusFilters";
            this.statusFilters.Size = new System.Drawing.Size(800, 22);
            this.statusFilters.TabIndex = 2;
            this.statusFilters.Text = "statusStrip1";
            // 
            // StatusInfoFilter
            // 
            this.StatusInfoFilter.Name = "StatusInfoFilter";
            this.StatusInfoFilter.Size = new System.Drawing.Size(86, 17);
            this.StatusInfoFilter.Text = "StatusInfoFilter";
            // 
            // SplitImage2
            // 
            this.SplitImage2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SplitImage2.Image = ((System.Drawing.Image)(resources.GetObject("SplitImage2.Image")));
            this.SplitImage2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SplitImage2.Name = "SplitImage2";
            this.SplitImage2.Size = new System.Drawing.Size(32, 20);
            this.SplitImage2.Text = "toolStripSplitButton1";
            // 
            // btnFilter1
            // 
            this.btnFilter1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFilter1.DoubleClickEnabled = true;
            this.btnFilter1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.btnFilter1.Image = ((System.Drawing.Image)(resources.GetObject("btnFilter1.Image")));
            this.btnFilter1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilter1.Name = "btnFilter1";
            this.btnFilter1.Size = new System.Drawing.Size(73, 20);
            this.btnFilter1.Text = "btnFilter1";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DoubleClickEnabled = true;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem4.Text = "toolStripMenuItem4";
            // 
            // btnFilter2
            // 
            this.btnFilter2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFilter2.Image = ((System.Drawing.Image)(resources.GetObject("btnFilter2.Image")));
            this.btnFilter2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilter2.Name = "btnFilter2";
            this.btnFilter2.Size = new System.Drawing.Size(70, 20);
            this.btnFilter2.Text = "btnFilter2";
            // 
            // btnFilter3
            // 
            this.btnFilter3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFilter3.DoubleClickEnabled = true;
            this.btnFilter3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem2});
            this.btnFilter3.Image = ((System.Drawing.Image)(resources.GetObject("btnFilter3.Image")));
            this.btnFilter3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilter3.Name = "btnFilter3";
            this.btnFilter3.Size = new System.Drawing.Size(73, 20);
            this.btnFilter3.Text = "btnFilter3";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.CheckOnClick = true;
            this.toolStripMenuItem5.DoubleClickEnabled = true;
            this.toolStripMenuItem5.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem5.Text = "2";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(80, 22);
            this.toolStripMenuItem6.Text = "3";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(80, 22);
            this.toolStripMenuItem7.Text = "4";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DoubleClickEnabled = true;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItem2.Text = "1";
            // 
            // btnFilter4
            // 
            this.btnFilter4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFilter4.Image = ((System.Drawing.Image)(resources.GetObject("btnFilter4.Image")));
            this.btnFilter4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFilter4.Name = "btnFilter4";
            this.btnFilter4.Size = new System.Drawing.Size(70, 20);
            this.btnFilter4.Text = "btnFilter4";
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusFilters);
            this.Controls.Add(this.statusMessenges);
            this.Controls.Add(this.txtbNameQuery);
            this.Controls.Add(this.txtbBodyQuery);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusMessenges.ResumeLayout(false);
            this.statusMessenges.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusFilters.ResumeLayout(false);
            this.statusFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbResultShow;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem administratorMenu;
        private System.Windows.Forms.StatusStrip statusMessenges;
        private System.Windows.Forms.ToolStripMenuItem writeModelsListMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem changeViewPanelviewMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem importFromTextFileMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusInfoMain;
        private System.Windows.Forms.ToolStripMenuItem createLocalDBMenuItem;
        private System.Windows.Forms.TextBox txtbBodyQuery;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelExtraInfo;
        private System.Windows.Forms.ToolStripDropDownButton SplitImage1;
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
        private System.Windows.Forms.ToolStripMenuItem loadData1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadData2ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusFilters;
        private System.Windows.Forms.ToolStripStatusLabel StatusInfoFilter;
        private System.Windows.Forms.ToolStripSplitButton btnFilter1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem updateFiltersMenuItem;
        private System.Windows.Forms.ToolStripSplitButton SplitImage2;
        private System.Windows.Forms.ToolStripDropDownButton btnFilter2;
        private System.Windows.Forms.ToolStripSplitButton btnFilter3;
        private System.Windows.Forms.ToolStripDropDownButton btnFilter4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem allColumnsInTableQueryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNewDBMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusApp;
    }
}

