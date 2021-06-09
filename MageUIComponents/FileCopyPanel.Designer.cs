namespace MageUIComponents
{
    partial class FileCopyPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel3 = new System.Windows.Forms.Panel();
            this.ResolveCacheInfoFilesCtl = new System.Windows.Forms.CheckBox();
            this.OverwriteExistingCtl = new System.Windows.Forms.CheckBox();
            this.prefixLeaderCtl = new System.Windows.Forms.TextBox();
            this.prefixLeaderLabelCtl = new System.Windows.Forms.Label();
            this.prefixColNameLabelCtl = new System.Windows.Forms.Label();
            this.prefixColNameCtl = new System.Windows.Forms.TextBox();
            this.usePrefixCtl = new System.Windows.Forms.CheckBox();
            this.CopyAllCtl = new System.Windows.Forms.Button();
            this.CopySelectedCtl = new System.Windows.Forms.Button();
            this.SelectDirectoryCtl = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.OutputDirectoryCtl = new System.Windows.Forms.TextBox();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.ResolveCacheInfoFilesCtl);
            this.panel3.Controls.Add(this.OverwriteExistingCtl);
            this.panel3.Controls.Add(this.prefixLeaderCtl);
            this.panel3.Controls.Add(this.prefixLeaderLabelCtl);
            this.panel3.Controls.Add(this.prefixColNameLabelCtl);
            this.panel3.Controls.Add(this.prefixColNameCtl);
            this.panel3.Controls.Add(this.usePrefixCtl);
            this.panel3.Controls.Add(this.CopyAllCtl);
            this.panel3.Controls.Add(this.CopySelectedCtl);
            this.panel3.Controls.Add(this.SelectDirectoryCtl);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.OutputDirectoryCtl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(7, 6);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1057, 79);
            this.panel3.TabIndex = 4;
            // 
            // ResolveCacheInfoFilesCtl
            // 
            this.ResolveCacheInfoFilesCtl.AutoSize = true;
            this.ResolveCacheInfoFilesCtl.Location = new System.Drawing.Point(729, 48);
            this.ResolveCacheInfoFilesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.ResolveCacheInfoFilesCtl.Name = "ResolveCacheInfoFilesCtl";
            this.ResolveCacheInfoFilesCtl.Size = new System.Drawing.Size(177, 21);
            this.ResolveCacheInfoFilesCtl.TabIndex = 14;
            this.ResolveCacheInfoFilesCtl.Text = "Resolve CacheInfo files";
            this.ResolveCacheInfoFilesCtl.UseVisualStyleBackColor = true;
            // 
            // OverwriteExistingCtl
            // 
            this.OverwriteExistingCtl.AutoSize = true;
            this.OverwriteExistingCtl.Location = new System.Drawing.Point(505, 48);
            this.OverwriteExistingCtl.Margin = new System.Windows.Forms.Padding(4);
            this.OverwriteExistingCtl.Name = "OverwriteExistingCtl";
            this.OverwriteExistingCtl.Size = new System.Drawing.Size(170, 21);
            this.OverwriteExistingCtl.TabIndex = 13;
            this.OverwriteExistingCtl.Text = "Overwrite existing files";
            this.OverwriteExistingCtl.UseVisualStyleBackColor = true;
            // 
            // prefixLeaderCtl
            // 
            this.prefixLeaderCtl.Location = new System.Drawing.Point(356, 51);
            this.prefixLeaderCtl.Margin = new System.Windows.Forms.Padding(4);
            this.prefixLeaderCtl.Name = "prefixLeaderCtl";
            this.prefixLeaderCtl.Size = new System.Drawing.Size(131, 22);
            this.prefixLeaderCtl.TabIndex = 12;
            // 
            // prefixLeaderLabelCtl
            // 
            this.prefixLeaderLabelCtl.AutoSize = true;
            this.prefixLeaderLabelCtl.Location = new System.Drawing.Point(354, 33);
            this.prefixLeaderLabelCtl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.prefixLeaderLabelCtl.Name = "prefixLeaderLabelCtl";
            this.prefixLeaderLabelCtl.Size = new System.Drawing.Size(76, 17);
            this.prefixLeaderLabelCtl.TabIndex = 11;
            this.prefixLeaderLabelCtl.Text = "with leader";
            // 
            // prefixColNameLabelCtl
            // 
            this.prefixColNameLabelCtl.AutoSize = true;
            this.prefixColNameLabelCtl.Location = new System.Drawing.Point(184, 33);
            this.prefixColNameLabelCtl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.prefixColNameLabelCtl.Name = "prefixColNameLabelCtl";
            this.prefixColNameLabelCtl.Size = new System.Drawing.Size(91, 17);
            this.prefixColNameLabelCtl.TabIndex = 10;
            this.prefixColNameLabelCtl.Text = "using column";
            // 
            // prefixColNameCtl
            // 
            this.prefixColNameCtl.Location = new System.Drawing.Point(187, 51);
            this.prefixColNameCtl.Margin = new System.Windows.Forms.Padding(4);
            this.prefixColNameCtl.Name = "prefixColNameCtl";
            this.prefixColNameCtl.Size = new System.Drawing.Size(155, 22);
            this.prefixColNameCtl.TabIndex = 9;
            // 
            // usePrefixCtl
            // 
            this.usePrefixCtl.AutoSize = true;
            this.usePrefixCtl.Checked = true;
            this.usePrefixCtl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.usePrefixCtl.Location = new System.Drawing.Point(8, 48);
            this.usePrefixCtl.Margin = new System.Windows.Forms.Padding(4);
            this.usePrefixCtl.Name = "usePrefixCtl";
            this.usePrefixCtl.Size = new System.Drawing.Size(180, 21);
            this.usePrefixCtl.TabIndex = 8;
            this.usePrefixCtl.Text = "Apply prefix to file name";
            this.usePrefixCtl.UseVisualStyleBackColor = true;
            this.usePrefixCtl.CheckedChanged += new System.EventHandler(this.UsePrefixCtl_CheckedChanged);
            // 
            // CopyAllCtl
            // 
            this.CopyAllCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CopyAllCtl.Location = new System.Drawing.Point(789, 7);
            this.CopyAllCtl.Margin = new System.Windows.Forms.Padding(4);
            this.CopyAllCtl.Name = "CopyAllCtl";
            this.CopyAllCtl.Size = new System.Drawing.Size(127, 28);
            this.CopyAllCtl.TabIndex = 7;
            this.CopyAllCtl.Text = "Copy All";
            this.CopyAllCtl.UseVisualStyleBackColor = true;
            this.CopyAllCtl.Click += new System.EventHandler(this.CopyAllCtl_Click);
            // 
            // CopySelectedCtl
            // 
            this.CopySelectedCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CopySelectedCtl.Location = new System.Drawing.Point(924, 7);
            this.CopySelectedCtl.Margin = new System.Windows.Forms.Padding(4);
            this.CopySelectedCtl.Name = "CopySelectedCtl";
            this.CopySelectedCtl.Size = new System.Drawing.Size(127, 28);
            this.CopySelectedCtl.TabIndex = 7;
            this.CopySelectedCtl.Text = "Copy Selected";
            this.CopySelectedCtl.UseVisualStyleBackColor = true;
            this.CopySelectedCtl.Click += new System.EventHandler(this.CopySelectedCtl_Click);
            // 
            // SelectDirectoryCtl
            // 
            this.SelectDirectoryCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectDirectoryCtl.Location = new System.Drawing.Point(729, 7);
            this.SelectDirectoryCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SelectDirectoryCtl.Name = "SelectDirectoryCtl";
            this.SelectDirectoryCtl.Size = new System.Drawing.Size(40, 28);
            this.SelectDirectoryCtl.TabIndex = 6;
            this.SelectDirectoryCtl.Text = "...";
            this.SelectDirectoryCtl.UseVisualStyleBackColor = true;
            this.SelectDirectoryCtl.Click += new System.EventHandler(this.SelectDirectoryCtl_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Destination Directory";
            // 
            // OutputDirectoryCtl
            // 
            this.OutputDirectoryCtl.AcceptsReturn = true;
            this.OutputDirectoryCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputDirectoryCtl.Location = new System.Drawing.Point(148, 10);
            this.OutputDirectoryCtl.Margin = new System.Windows.Forms.Padding(4);
            this.OutputDirectoryCtl.Name = "OutputDirectoryCtl";
            this.OutputDirectoryCtl.Size = new System.Drawing.Size(572, 22);
            this.OutputDirectoryCtl.TabIndex = 1;
            // 
            // FileCopyPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FileCopyPanel";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(1071, 91);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button SelectDirectoryCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OutputDirectoryCtl;
        private System.Windows.Forms.Button CopyAllCtl;
        private System.Windows.Forms.Button CopySelectedCtl;
        private System.Windows.Forms.CheckBox usePrefixCtl;
        private System.Windows.Forms.TextBox prefixColNameCtl;
        private System.Windows.Forms.Label prefixColNameLabelCtl;
        private System.Windows.Forms.TextBox prefixLeaderCtl;
        private System.Windows.Forms.Label prefixLeaderLabelCtl;
        private System.Windows.Forms.CheckBox OverwriteExistingCtl;
        private System.Windows.Forms.CheckBox ResolveCacheInfoFilesCtl;
    }
}
