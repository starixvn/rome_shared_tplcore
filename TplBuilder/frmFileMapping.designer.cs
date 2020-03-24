namespace TplBuilder
{
    partial class frmFileMapping
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFileMapping));
            this.grid = new System.Windows.Forms.DataGridView();
            this.colImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDestination = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMappingErrorType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblLinkTotal = new System.Windows.Forms.Label();
            this.lblBadLinkTotal = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chbShowFullPath = new System.Windows.Forms.CheckBox();
            this.chbOnlyShowBadLinks = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colImage,
            this.colSource,
            this.colDestination,
            this.colMappingErrorType});
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 45);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(538, 349);
            this.grid.TabIndex = 0;
            // 
            // colImage
            // 
            this.colImage.HeaderText = "";
            this.colImage.Name = "colImage";
            this.colImage.ReadOnly = true;
            this.colImage.Width = 24;
            // 
            // colSource
            // 
            this.colSource.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colSource.HeaderText = "Source (HTM)";
            this.colSource.Name = "colSource";
            this.colSource.ReadOnly = true;
            // 
            // colDestination
            // 
            this.colDestination.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDestination.HeaderText = "Destination (HTM.CS)";
            this.colDestination.Name = "colDestination";
            this.colDestination.ReadOnly = true;
            // 
            // colMappingErrorType
            // 
            this.colMappingErrorType.HeaderText = "ErrorType";
            this.colMappingErrorType.Name = "colMappingErrorType";
            this.colMappingErrorType.ReadOnly = true;
            this.colMappingErrorType.Visible = false;
            // 
            // lblLinkTotal
            // 
            this.lblLinkTotal.AutoSize = true;
            this.lblLinkTotal.Location = new System.Drawing.Point(3, 8);
            this.lblLinkTotal.Name = "lblLinkTotal";
            this.lblLinkTotal.Size = new System.Drawing.Size(74, 13);
            this.lblLinkTotal.TabIndex = 1;
            this.lblLinkTotal.Text = "Link Total: {0}";
            // 
            // lblBadLinkTotal
            // 
            this.lblBadLinkTotal.AutoSize = true;
            this.lblBadLinkTotal.ForeColor = System.Drawing.Color.Red;
            this.lblBadLinkTotal.Location = new System.Drawing.Point(3, 26);
            this.lblBadLinkTotal.Name = "lblBadLinkTotal";
            this.lblBadLinkTotal.Size = new System.Drawing.Size(96, 13);
            this.lblBadLinkTotal.TabIndex = 2;
            this.lblBadLinkTotal.Text = "Bad Link Total: {0}";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chbShowFullPath);
            this.panel1.Controls.Add(this.chbOnlyShowBadLinks);
            this.panel1.Controls.Add(this.lblLinkTotal);
            this.panel1.Controls.Add(this.lblBadLinkTotal);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(538, 45);
            this.panel1.TabIndex = 3;
            // 
            // chbShowFullPath
            // 
            this.chbShowFullPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbShowFullPath.AutoSize = true;
            this.chbShowFullPath.Location = new System.Drawing.Point(415, 7);
            this.chbShowFullPath.Name = "chbShowFullPath";
            this.chbShowFullPath.Size = new System.Drawing.Size(93, 17);
            this.chbShowFullPath.TabIndex = 4;
            this.chbShowFullPath.Text = "Show full path";
            this.chbShowFullPath.UseVisualStyleBackColor = true;
            this.chbShowFullPath.CheckedChanged += new System.EventHandler(this.chbShowFullPath_CheckedChanged);
            // 
            // chbOnlyShowBadLinks
            // 
            this.chbOnlyShowBadLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chbOnlyShowBadLinks.AutoSize = true;
            this.chbOnlyShowBadLinks.Location = new System.Drawing.Point(415, 25);
            this.chbOnlyShowBadLinks.Name = "chbOnlyShowBadLinks";
            this.chbOnlyShowBadLinks.Size = new System.Drawing.Size(120, 17);
            this.chbOnlyShowBadLinks.TabIndex = 3;
            this.chbOnlyShowBadLinks.Text = "Only show bad links";
            this.chbOnlyShowBadLinks.UseVisualStyleBackColor = true;
            this.chbOnlyShowBadLinks.CheckedChanged += new System.EventHandler(this.chbOnlyShowBadLinks_CheckedChanged);
            // 
            // frmFileMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 394);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFileMapping";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Mapping Table";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Label lblLinkTotal;
        private System.Windows.Forms.Label lblBadLinkTotal;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chbOnlyShowBadLinks;
        private System.Windows.Forms.DataGridViewImageColumn colImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDestination;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMappingErrorType;
        private System.Windows.Forms.CheckBox chbShowFullPath;
    }
}