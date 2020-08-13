namespace FlexibleDBMS
{
    partial class SelectDBForm
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
            this.tbResultShow = new System.Windows.Forms.TextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.cmbSQLProvider = new System.Windows.Forms.ComboBox();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.cmbDataBases = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbTables = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SuspendLayout();
            // 
            // tbResultShow
            // 
            this.tbResultShow.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResultShow.Location = new System.Drawing.Point(0, 125);
            this.tbResultShow.Multiline = true;
            this.tbResultShow.Name = "tbResultShow";
            this.tbResultShow.ReadOnly = true;
            this.tbResultShow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbResultShow.Size = new System.Drawing.Size(800, 300);
            this.tbResultShow.TabIndex = 10;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(12, 99);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(121, 20);
            this.btnCheck.TabIndex = 6;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            // 
            // cmbSQLProvider
            // 
            this.cmbSQLProvider.FormattingEnabled = true;
            this.cmbSQLProvider.Location = new System.Drawing.Point(12, 38);
            this.cmbSQLProvider.Name = "cmbSQLProvider";
            this.cmbSQLProvider.Size = new System.Drawing.Size(121, 21);
            this.cmbSQLProvider.TabIndex = 0;
            // 
            // tbHost
            // 
            this.tbHost.Location = new System.Drawing.Point(145, 12);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(112, 20);
            this.tbHost.TabIndex = 2;
            this.tbHost.Text = "Host";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(269, 12);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(110, 20);
            this.tbPort.TabIndex = 3;
            this.tbPort.Text = "Port";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(269, 38);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(110, 20);
            this.tbPassword.TabIndex = 5;
            this.tbPassword.Text = "Password";
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(145, 38);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(112, 20);
            this.tbUserName.TabIndex = 4;
            this.tbUserName.Text = "UserName";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(12, 12);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(121, 20);
            this.tbName.TabIndex = 1;
            this.tbName.Text = "Name";
            // 
            // cmbDataBases
            // 
            this.cmbDataBases.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbDataBases.FormattingEnabled = true;
            this.cmbDataBases.Location = new System.Drawing.Point(393, 12);
            this.cmbDataBases.Name = "cmbDataBases";
            this.cmbDataBases.Size = new System.Drawing.Size(121, 21);
            this.cmbDataBases.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(145, 99);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 20);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(269, 99);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 20);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmbTables
            // 
            this.cmbTables.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTables.FormattingEnabled = true;
            this.cmbTables.Location = new System.Drawing.Point(393, 39);
            this.cmbTables.Name = "cmbTables";
            this.cmbTables.Size = new System.Drawing.Size(121, 21);
            this.cmbTables.TabIndex = 11;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SelectDBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.cmbTables);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbDataBases);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.tbUserName);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.tbHost);
            this.Controls.Add(this.cmbSQLProvider);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.tbResultShow);
            this.Name = "SelectDBForm";
            this.Text = "SelectDB";
            this.Load += new System.EventHandler(this.SelectDBForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbDataBases;
        private System.Windows.Forms.ComboBox cmbSQLProvider;
        private System.Windows.Forms.ComboBox cmbTables;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbResultShow;
        private System.Windows.Forms.TextBox tbUserName;

        #endregion
    }
}