namespace MageUIComponents {
    partial class FileCopyPanel {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			this.panel3 = new System.Windows.Forms.Panel();
			this.OverwriteExistingCtl = new System.Windows.Forms.CheckBox();
			this.prefixLeaderCtl = new System.Windows.Forms.TextBox();
			this.prefixLeaderLabelCtl = new System.Windows.Forms.Label();
			this.prefixColNameLabelCtl = new System.Windows.Forms.Label();
			this.prefixColNameCtl = new System.Windows.Forms.TextBox();
			this.usePrefixCtl = new System.Windows.Forms.CheckBox();
			this.CopyAllCtl = new System.Windows.Forms.Button();
			this.CopySelectedCtl = new System.Windows.Forms.Button();
			this.SelectFolderCtl = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.OutputFolderCtl = new System.Windows.Forms.TextBox();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel3
			// 
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel3.Controls.Add(this.OverwriteExistingCtl);
			this.panel3.Controls.Add(this.prefixLeaderCtl);
			this.panel3.Controls.Add(this.prefixLeaderLabelCtl);
			this.panel3.Controls.Add(this.prefixColNameLabelCtl);
			this.panel3.Controls.Add(this.prefixColNameCtl);
			this.panel3.Controls.Add(this.usePrefixCtl);
			this.panel3.Controls.Add(this.CopyAllCtl);
			this.panel3.Controls.Add(this.CopySelectedCtl);
			this.panel3.Controls.Add(this.SelectFolderCtl);
			this.panel3.Controls.Add(this.label1);
			this.panel3.Controls.Add(this.OutputFolderCtl);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(5, 5);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(634, 64);
			this.panel3.TabIndex = 4;
			// 
			// OverwriteExistingCtl
			// 
			this.OverwriteExistingCtl.AutoSize = true;
			this.OverwriteExistingCtl.Location = new System.Drawing.Point(566, 36);
			this.OverwriteExistingCtl.Name = "OverwriteExistingCtl";
			this.OverwriteExistingCtl.Size = new System.Drawing.Size(130, 17);
			this.OverwriteExistingCtl.TabIndex = 13;
			this.OverwriteExistingCtl.Text = "Overwrite existing files";
			this.OverwriteExistingCtl.UseVisualStyleBackColor = true;
			// 
			// prefixLeaderCtl
			// 
			this.prefixLeaderCtl.Location = new System.Drawing.Point(399, 34);
			this.prefixLeaderCtl.Name = "prefixLeaderCtl";
			this.prefixLeaderCtl.Size = new System.Drawing.Size(140, 20);
			this.prefixLeaderCtl.TabIndex = 12;
			// 
			// prefixLeaderLabelCtl
			// 
			this.prefixLeaderLabelCtl.AutoSize = true;
			this.prefixLeaderLabelCtl.Location = new System.Drawing.Point(335, 38);
			this.prefixLeaderLabelCtl.Name = "prefixLeaderLabelCtl";
			this.prefixLeaderLabelCtl.Size = new System.Drawing.Size(58, 13);
			this.prefixLeaderLabelCtl.TabIndex = 11;
			this.prefixLeaderLabelCtl.Text = "with leader";
			// 
			// prefixColNameLabelCtl
			// 
			this.prefixColNameLabelCtl.AutoSize = true;
			this.prefixColNameLabelCtl.Location = new System.Drawing.Point(138, 38);
			this.prefixColNameLabelCtl.Name = "prefixColNameLabelCtl";
			this.prefixColNameLabelCtl.Size = new System.Drawing.Size(69, 13);
			this.prefixColNameLabelCtl.TabIndex = 10;
			this.prefixColNameLabelCtl.Text = "using column";
			// 
			// prefixColNameCtl
			// 
			this.prefixColNameCtl.Location = new System.Drawing.Point(212, 34);
			this.prefixColNameCtl.Name = "prefixColNameCtl";
			this.prefixColNameCtl.Size = new System.Drawing.Size(117, 20);
			this.prefixColNameCtl.TabIndex = 9;
			// 
			// usePrefixCtl
			// 
			this.usePrefixCtl.AutoSize = true;
			this.usePrefixCtl.Checked = true;
			this.usePrefixCtl.CheckState = System.Windows.Forms.CheckState.Checked;
			this.usePrefixCtl.Location = new System.Drawing.Point(6, 36);
			this.usePrefixCtl.Name = "usePrefixCtl";
			this.usePrefixCtl.Size = new System.Drawing.Size(137, 17);
			this.usePrefixCtl.TabIndex = 8;
			this.usePrefixCtl.Text = "Apply prefix to file name";
			this.usePrefixCtl.UseVisualStyleBackColor = true;
			this.usePrefixCtl.CheckedChanged += new System.EventHandler(this.usePrefixCtl_CheckedChanged);
			// 
			// CopyAllCtl
			// 
			this.CopyAllCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CopyAllCtl.Location = new System.Drawing.Point(433, 6);
			this.CopyAllCtl.Name = "CopyAllCtl";
			this.CopyAllCtl.Size = new System.Drawing.Size(95, 23);
			this.CopyAllCtl.TabIndex = 7;
			this.CopyAllCtl.Text = "Copy All";
			this.CopyAllCtl.UseVisualStyleBackColor = true;
			this.CopyAllCtl.Click += new System.EventHandler(this.CopyAllCtl_Click);
			// 
			// CopySelectedCtl
			// 
			this.CopySelectedCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CopySelectedCtl.Location = new System.Drawing.Point(534, 6);
			this.CopySelectedCtl.Name = "CopySelectedCtl";
			this.CopySelectedCtl.Size = new System.Drawing.Size(95, 23);
			this.CopySelectedCtl.TabIndex = 7;
			this.CopySelectedCtl.Text = "Copy Selected";
			this.CopySelectedCtl.UseVisualStyleBackColor = true;
			this.CopySelectedCtl.Click += new System.EventHandler(this.CopySelectedCtl_Click);
			// 
			// SelectFolderCtl
			// 
			this.SelectFolderCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SelectFolderCtl.Location = new System.Drawing.Point(388, 6);
			this.SelectFolderCtl.Name = "SelectFolderCtl";
			this.SelectFolderCtl.Size = new System.Drawing.Size(30, 23);
			this.SelectFolderCtl.TabIndex = 6;
			this.SelectFolderCtl.Text = "...";
			this.SelectFolderCtl.UseVisualStyleBackColor = true;
			this.SelectFolderCtl.Click += new System.EventHandler(this.SelectFolderCtl_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Destination Folder";
			// 
			// OutputFolderCtl
			// 
			this.OutputFolderCtl.AcceptsReturn = true;
			this.OutputFolderCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.OutputFolderCtl.Location = new System.Drawing.Point(100, 8);
			this.OutputFolderCtl.Name = "OutputFolderCtl";
			this.OutputFolderCtl.Size = new System.Drawing.Size(282, 20);
			this.OutputFolderCtl.TabIndex = 1;
			// 
			// FileCopyPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel3);
			this.Name = "FileCopyPanel";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.Size = new System.Drawing.Size(644, 74);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button SelectFolderCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OutputFolderCtl;
        private System.Windows.Forms.Button CopyAllCtl;
        private System.Windows.Forms.Button CopySelectedCtl;
        private System.Windows.Forms.CheckBox usePrefixCtl;
        private System.Windows.Forms.TextBox prefixColNameCtl;
        private System.Windows.Forms.Label prefixColNameLabelCtl;
        private System.Windows.Forms.TextBox prefixLeaderCtl;
        private System.Windows.Forms.Label prefixLeaderLabelCtl;
		private System.Windows.Forms.CheckBox OverwriteExistingCtl;
    }
}
