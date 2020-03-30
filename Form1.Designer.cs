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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.administratorMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.createLocalDBMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromTextFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeModelsListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analysisDataMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.dataMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.changeViewPanelviewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.analysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(-1, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(801, 394);
            this.textBox1.TabIndex = 1;
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
            this.loadDataMenuItem});
            this.analysisDataMenu.Name = "analysisDataMenu";
            this.analysisDataMenu.Size = new System.Drawing.Size(62, 20);
            this.analysisDataMenu.Text = "Analysis";
            // 
            // dataMenu
            // 
            this.dataMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeViewPanelviewMenuItem});
            this.dataMenu.Name = "dataMenu";
            this.dataMenu.Size = new System.Drawing.Size(43, 20);
            this.dataMenu.Text = "Data";
            // 
            // changeViewPanelviewMenuItem
            // 
            this.changeViewPanelviewMenuItem.Name = "changeViewPanelviewMenuItem";
            this.changeViewPanelviewMenuItem.Size = new System.Drawing.Size(180, 22);
            this.changeViewPanelviewMenuItem.Text = "Show table";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel1
            // 
            this.StatusLabel1.Name = "StatusLabel1";
            this.StatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.StatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(803, 400);
            this.panel1.TabIndex = 5;
            // 
            // analysisToolStripMenuItem
            // 
            this.analysisToolStripMenuItem.Name = "analysisToolStripMenuItem";
            this.analysisToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.analysisToolStripMenuItem.Text = "Analysis";
            // 
            // loadDataToolStripMenuItem
            // 
            this.loadDataMenuItem.Name = "loadDataToolStripMenuItem";
            this.loadDataMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadDataMenuItem.Text = "Load data";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
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

        private System.Windows.Forms.TextBox textBox1;
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
    }
}

