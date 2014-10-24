namespace MageConcatenator
{
    partial class MageConcatenator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MageConcatenator));
            this.pnlControlsAndStatus = new System.Windows.Forms.Panel();
            this.cmdAbout = new System.Windows.Forms.Button();
            this.chkAddFileName = new System.Windows.Forms.CheckBox();
            this.FolderDestinationPanel1 = new MageUIComponents.FolderDestinationPanel();
            this.statusPanel1 = new MageDisplayLib.StatusPanel();
            this.ProcessAllFilesCtl = new System.Windows.Forms.Button();
            this.ProcessSelectedFilesCtl = new System.Windows.Forms.Button();
            this.FileListDisplayControl = new MageDisplayLib.GridViewDisplayControl();
            this.LocalFolderPanel1 = new MageUIComponents.LocalFolderPanel();
            this.pnlControlsAndStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControlsAndStatus
            // 
            this.pnlControlsAndStatus.Controls.Add(this.cmdAbout);
            this.pnlControlsAndStatus.Controls.Add(this.chkAddFileName);
            this.pnlControlsAndStatus.Controls.Add(this.FolderDestinationPanel1);
            this.pnlControlsAndStatus.Controls.Add(this.statusPanel1);
            this.pnlControlsAndStatus.Controls.Add(this.ProcessAllFilesCtl);
            this.pnlControlsAndStatus.Controls.Add(this.ProcessSelectedFilesCtl);
            this.pnlControlsAndStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControlsAndStatus.Location = new System.Drawing.Point(0, 412);
            this.pnlControlsAndStatus.Name = "pnlControlsAndStatus";
            this.pnlControlsAndStatus.Size = new System.Drawing.Size(1274, 159);
            this.pnlControlsAndStatus.TabIndex = 15;
            // 
            // cmdAbout
            // 
            this.cmdAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAbout.Location = new System.Drawing.Point(1191, 10);
            this.cmdAbout.Margin = new System.Windows.Forms.Padding(4);
            this.cmdAbout.Name = "cmdAbout";
            this.cmdAbout.Size = new System.Drawing.Size(77, 28);
            this.cmdAbout.TabIndex = 27;
            this.cmdAbout.Text = "About";
            this.cmdAbout.UseVisualStyleBackColor = true;
            this.cmdAbout.Click += new System.EventHandler(this.cmdAbout_Click);
            // 
            // chkAddFileName
            // 
            this.chkAddFileName.AutoSize = true;
            this.chkAddFileName.Checked = true;
            this.chkAddFileName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddFileName.Location = new System.Drawing.Point(993, 81);
            this.chkAddFileName.Name = "chkAddFileName";
            this.chkAddFileName.Size = new System.Drawing.Size(211, 21);
            this.chkAddFileName.TabIndex = 25;
            this.chkAddFileName.Text = "Add Filename as first column";
            this.chkAddFileName.UseVisualStyleBackColor = true;
            // 
            // FolderDestinationPanel1
            // 
            this.FolderDestinationPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FolderDestinationPanel1.Location = new System.Drawing.Point(5, 10);
            this.FolderDestinationPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.FolderDestinationPanel1.Name = "FolderDestinationPanel1";
            this.FolderDestinationPanel1.OutputFile = "";
            this.FolderDestinationPanel1.OutputFolder = "C:\\Data\\Junk";
            this.FolderDestinationPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.FolderDestinationPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FolderDestinationPanel1.Size = new System.Drawing.Size(983, 84);
            this.FolderDestinationPanel1.TabIndex = 23;
            // 
            // statusPanel1
            // 
            this.statusPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.statusPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPanel1.EnableCancel = true;
            this.statusPanel1.Location = new System.Drawing.Point(5, 104);
            this.statusPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.statusPanel1.Name = "statusPanel1";
            this.statusPanel1.OwnerControl = this;
            this.statusPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.statusPanel1.ShowCancel = true;
            this.statusPanel1.Size = new System.Drawing.Size(1264, 50);
            this.statusPanel1.TabIndex = 24;
            // 
            // ProcessAllFilesCtl
            // 
            this.ProcessAllFilesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessAllFilesCtl.Location = new System.Drawing.Point(993, 46);
            this.ProcessAllFilesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.ProcessAllFilesCtl.Name = "ProcessAllFilesCtl";
            this.ProcessAllFilesCtl.Size = new System.Drawing.Size(187, 28);
            this.ProcessAllFilesCtl.TabIndex = 22;
            this.ProcessAllFilesCtl.Text = "&Process All Files";
            this.ProcessAllFilesCtl.UseVisualStyleBackColor = true;
            this.ProcessAllFilesCtl.Click += new System.EventHandler(this.ProcessAllFilesCtl_Click);
            // 
            // ProcessSelectedFilesCtl
            // 
            this.ProcessSelectedFilesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessSelectedFilesCtl.Location = new System.Drawing.Point(993, 10);
            this.ProcessSelectedFilesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.ProcessSelectedFilesCtl.Name = "ProcessSelectedFilesCtl";
            this.ProcessSelectedFilesCtl.Size = new System.Drawing.Size(187, 28);
            this.ProcessSelectedFilesCtl.TabIndex = 21;
            this.ProcessSelectedFilesCtl.Text = "Process Selected &Files";
            this.ProcessSelectedFilesCtl.UseVisualStyleBackColor = true;
            this.ProcessSelectedFilesCtl.Click += new System.EventHandler(this.ProcessSelectedFilesCtl_Click);
            // 
            // FileListDisplayControl
            // 
            this.FileListDisplayControl.AllowDisableShiftClickMode = true;
            this.FileListDisplayControl.AutoSizeColumnWidths = false;
            this.FileListDisplayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileListDisplayControl.HeaderVisible = true;
            this.FileListDisplayControl.ItemBlockSize = 100;
            this.FileListDisplayControl.Location = new System.Drawing.Point(0, 93);
            this.FileListDisplayControl.Margin = new System.Windows.Forms.Padding(5);
            this.FileListDisplayControl.MultiSelect = true;
            this.FileListDisplayControl.Name = "FileListDisplayControl";
            this.FileListDisplayControl.Notice = "";
            this.FileListDisplayControl.PageTitle = "Title";
            this.FileListDisplayControl.Size = new System.Drawing.Size(1274, 319);
            this.FileListDisplayControl.TabIndex = 17;
            // 
            // LocalFolderPanel1
            // 
            this.LocalFolderPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.LocalFolderPanel1.FileNameFilter = "syn.txt";
            this.LocalFolderPanel1.FileSelectionMode = "FileSearch";
            this.LocalFolderPanel1.Folder = "C:\\Data\\syn";
            this.LocalFolderPanel1.Location = new System.Drawing.Point(0, 0);
            this.LocalFolderPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.LocalFolderPanel1.MostRecentFolder = "";
            this.LocalFolderPanel1.Name = "LocalFolderPanel1";
            this.LocalFolderPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.LocalFolderPanel1.SearchInSubfolders = "No";
            this.LocalFolderPanel1.Size = new System.Drawing.Size(1274, 93);
            this.LocalFolderPanel1.SubfolderSearchName = "*";
            this.LocalFolderPanel1.TabIndex = 8;
            // 
            // MageConcatenator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 571);
            this.Controls.Add(this.FileListDisplayControl);
            this.Controls.Add(this.pnlControlsAndStatus);
            this.Controls.Add(this.LocalFolderPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MageConcatenator";
            this.Text = "Mage File Concatenator";
            this.pnlControlsAndStatus.ResumeLayout(false);
            this.pnlControlsAndStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MageUIComponents.LocalFolderPanel LocalFolderPanel1;
        private System.Windows.Forms.Panel pnlControlsAndStatus;
        private MageUIComponents.FolderDestinationPanel FolderDestinationPanel1;
        private MageDisplayLib.StatusPanel statusPanel1;
        private MageDisplayLib.GridViewDisplayControl FileListDisplayControl;
        private System.Windows.Forms.Button ProcessAllFilesCtl;
        private System.Windows.Forms.Button ProcessSelectedFilesCtl;
        private System.Windows.Forms.CheckBox chkAddFileName;
        private System.Windows.Forms.Button cmdAbout;
    }
}