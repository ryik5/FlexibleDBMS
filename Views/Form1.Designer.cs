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
            this.connectToSQLServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.importFromTextFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeModelsListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.changeViewPanelviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.updateFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.recoverDataTableAfterQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.allColumnsInTableQueryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.exportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.loadData0MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadData1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadData2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.lookForNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookForFamiliyNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookForOrganizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.queriesExtraMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.removeQueryExtraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useApplicationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusMessenges = new System.Windows.Forms.StatusStrip();
            this.StatusApp = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusInfoMain = new System.Windows.Forms.ToolStripStatusLabel();
            this.SplitImage1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.StatusLabelExtraInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusFilters = new System.Windows.Forms.StatusStrip();
            this.StatusInfoFilter = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.txtBodyQuery = new System.Windows.Forms.TextBox();
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
            this.queryMenu,
            this.helpMenu});
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
            this.connectToSQLServerToolStripMenuItem,
            this.recentConnectionToolStripMenuItem,
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
            this.createLocalDBMenuItem.Size = new System.Drawing.Size(191, 22);
            this.createLocalDBMenuItem.Text = "Create local DB";
            // 
            // selectNewDBMenuItem
            // 
            this.selectNewDBMenuItem.Name = "selectNewDBMenuItem";
            this.selectNewDBMenuItem.Size = new System.Drawing.Size(191, 22);
            this.selectNewDBMenuItem.Text = "Select new DB";
            // 
            // connectToSQLServerToolStripMenuItem
            // 
            this.connectToSQLServerToolStripMenuItem.Name = "connectToSQLServerToolStripMenuItem";
            this.connectToSQLServerToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.connectToSQLServerToolStripMenuItem.Text = "Connect to SQL server";
            // 
            // recentConnectionToolStripMenuItem
            // 
            this.recentConnectionToolStripMenuItem.Name = "recentConnectionToolStripMenuItem";
            this.recentConnectionToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.recentConnectionToolStripMenuItem.Text = "Recent connection";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(188, 6);
            // 
            // importFromTextFileMenuItem
            // 
            this.importFromTextFileMenuItem.Name = "importFromTextFileMenuItem";
            this.importFromTextFileMenuItem.Size = new System.Drawing.Size(191, 22);
            this.importFromTextFileMenuItem.Text = "Import from text file";
            // 
            // writeModelsListMenuItem
            // 
            this.writeModelsListMenuItem.Name = "writeModelsListMenuItem";
            this.writeModelsListMenuItem.Size = new System.Drawing.Size(191, 22);
            this.writeModelsListMenuItem.Text = "Write models List";
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeViewPanelviewMenuItem,
            this.toolStripSeparator8,
            this.updateFiltersMenuItem,
            this.toolStripSeparator9,
            this.recoverDataTableAfterQueryMenuItem,
            this.toolStripSeparator6,
            this.allColumnsInTableQueryMenuItem,
            this.toolStripSeparator7,
            this.exportMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "View";
            // 
            // changeViewPanelviewMenuItem
            // 
            this.changeViewPanelviewMenuItem.Name = "changeViewPanelviewMenuItem";
            this.changeViewPanelviewMenuItem.Size = new System.Drawing.Size(234, 22);
            this.changeViewPanelviewMenuItem.Text = "Change view";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(231, 6);
            // 
            // updateFiltersMenuItem
            // 
            this.updateFiltersMenuItem.Name = "updateFiltersMenuItem";
            this.updateFiltersMenuItem.Size = new System.Drawing.Size(234, 22);
            this.updateFiltersMenuItem.Text = "Update filters";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(231, 6);
            // 
            // recoverDataTableAfterQueryMenuItem
            // 
            this.recoverDataTableAfterQueryMenuItem.Name = "recoverDataTableAfterQueryMenuItem";
            this.recoverDataTableAfterQueryMenuItem.Size = new System.Drawing.Size(234, 22);
            this.recoverDataTableAfterQueryMenuItem.Text = "Recover DataTable After Query";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(231, 6);
            // 
            // allColumnsInTableQueryMenuItem
            // 
            this.allColumnsInTableQueryMenuItem.Name = "allColumnsInTableQueryMenuItem";
            this.allColumnsInTableQueryMenuItem.Size = new System.Drawing.Size(234, 22);
            this.allColumnsInTableQueryMenuItem.Text = "All Columns in the table";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(231, 6);
            // 
            // exportMenuItem
            // 
            this.exportMenuItem.Name = "exportMenuItem";
            this.exportMenuItem.Size = new System.Drawing.Size(234, 22);
            this.exportMenuItem.Text = "Export";
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
            this.getFIOMenuItem,
            this.getEnterpriseMenuItem,
            this.toolStripSeparator3,
            this.loadData0MenuItem,
            this.loadData1MenuItem,
            this.loadData2MenuItem,
            this.toolStripSeparator10,
            this.lookForNumberToolStripMenuItem,
            this.lookForFamiliyNameToolStripMenuItem,
            this.lookForOrganizationToolStripMenuItem});
            this.queriesStandartMenu.Name = "queriesStandartMenu";
            this.queriesStandartMenu.Size = new System.Drawing.Size(180, 22);
            this.queriesStandartMenu.Text = "Standart queries";
            // 
            // schemeLocalDBMenuItem
            // 
            this.schemeLocalDBMenuItem.Name = "schemeLocalDBMenuItem";
            this.schemeLocalDBMenuItem.Size = new System.Drawing.Size(189, 22);
            this.schemeLocalDBMenuItem.Text = "Scheme local DB";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // analysisToolStripMenuItem1
            // 
            this.analysisToolStripMenuItem1.Name = "analysisToolStripMenuItem1";
            this.analysisToolStripMenuItem1.Size = new System.Drawing.Size(189, 22);
            this.analysisToolStripMenuItem1.Text = "Analysis";
            // 
            // getFIOMenuItem
            // 
            this.getFIOMenuItem.Name = "getFIOMenuItem";
            this.getFIOMenuItem.Size = new System.Drawing.Size(189, 22);
            this.getFIOMenuItem.Text = "Get FIO";
            // 
            // getEnterpriseMenuItem
            // 
            this.getEnterpriseMenuItem.Name = "getEnterpriseMenuItem";
            this.getEnterpriseMenuItem.Size = new System.Drawing.Size(189, 22);
            this.getEnterpriseMenuItem.Text = "Get Enterprise";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(186, 6);
            // 
            // loadData0MenuItem
            // 
            this.loadData0MenuItem.Name = "loadData0MenuItem";
            this.loadData0MenuItem.Size = new System.Drawing.Size(189, 22);
            this.loadData0MenuItem.Text = "Load data";
            // 
            // loadData1MenuItem
            // 
            this.loadData1MenuItem.Name = "loadData1MenuItem";
            this.loadData1MenuItem.Size = new System.Drawing.Size(189, 22);
            this.loadData1MenuItem.Text = "Load data1";
            // 
            // loadData2MenuItem
            // 
            this.loadData2MenuItem.Name = "loadData2MenuItem";
            this.loadData2MenuItem.Size = new System.Drawing.Size(189, 22);
            this.loadData2MenuItem.Text = "Load data 2";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(186, 6);
            // 
            // lookForNumberToolStripMenuItem
            // 
            this.lookForNumberToolStripMenuItem.Name = "lookForNumberToolStripMenuItem";
            this.lookForNumberToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.lookForNumberToolStripMenuItem.Text = "Look for Number";
            // 
            // lookForFamiliyNameToolStripMenuItem
            // 
            this.lookForFamiliyNameToolStripMenuItem.Name = "lookForFamiliyNameToolStripMenuItem";
            this.lookForFamiliyNameToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.lookForFamiliyNameToolStripMenuItem.Text = "Look for Familiy";
            // 
            // lookForOrganizationToolStripMenuItem
            // 
            this.lookForOrganizationToolStripMenuItem.Name = "lookForOrganizationToolStripMenuItem";
            this.lookForOrganizationToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.lookForOrganizationToolStripMenuItem.Text = "Look for Organization";
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
            // removeQueryExtraMenuItem
            // 
            this.removeQueryExtraMenuItem.Name = "removeQueryExtraMenuItem";
            this.removeQueryExtraMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeQueryExtraMenuItem.Text = "Remove";
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem,
            this.useApplicationMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(44, 20);
            this.helpMenu.Text = "Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(154, 22);
            this.aboutMenuItem.Text = "About";
            // 
            // useApplicationMenuItem
            // 
            this.useApplicationMenuItem.Name = "useApplicationMenuItem";
            this.useApplicationMenuItem.Size = new System.Drawing.Size(154, 22);
            this.useApplicationMenuItem.Text = "UseApplication";
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
            this.progressBar});
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
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // txtBodyQuery
            // 
            this.txtBodyQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBodyQuery.Location = new System.Drawing.Point(502, 3);
            this.txtBodyQuery.Name = "txtBodyQuery";
            this.txtBodyQuery.Size = new System.Drawing.Size(296, 20);
            this.txtBodyQuery.TabIndex = 6;
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
            this.Controls.Add(this.txtBodyQuery);
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
        private System.Windows.Forms.TextBox txtBodyQuery;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelExtraInfo;
        private System.Windows.Forms.ToolStripDropDownButton SplitImage1;
        private System.Windows.Forms.ToolStripMenuItem queryMenu;
        private System.Windows.Forms.ToolStripMenuItem addQueryExtraMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queriesStandartMenu;
        private System.Windows.Forms.ToolStripMenuItem analysisToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadData0MenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem loadData1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadData2MenuItem;
        private System.Windows.Forms.StatusStrip statusFilters;
        private System.Windows.Forms.ToolStripStatusLabel StatusInfoFilter;
        private System.Windows.Forms.ToolStripMenuItem updateFiltersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allColumnsInTableQueryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNewDBMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusApp;
        private System.Windows.Forms.ToolStripMenuItem exportMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recoverDataTableAfterQueryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useApplicationMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem lookForNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lookForFamiliyNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lookForOrganizationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToSQLServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentConnectionToolStripMenuItem;
    }
}

