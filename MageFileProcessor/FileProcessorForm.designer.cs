namespace MageFileProcessor {
    partial class FileProcessorForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileProcessorForm));
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.statusPanel1 = new MageDisplayLib.StatusPanel();
            this.FileProcessingPanel1 = new MageUIComponents.FileProcessingPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.FilterOutputTabs = new System.Windows.Forms.TabControl();
            this.CopyFilesTabPage = new System.Windows.Forms.TabPage();
            this.FileCopyPanel1 = new MageUIComponents.FileCopyPanel();
            this.ProcessFilesToLocalTabPage = new System.Windows.Forms.TabPage();
            this.FolderDestinationPanel1 = new MageUIComponents.FolderDestinationPanel();
            this.ProcessFileToSQLiteDBTabPage = new System.Windows.Forms.TabPage();
            this.SQLiteDestinationPanel1 = new MageUIComponents.SQLiteDestinationPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.JobListDisplayControl = new MageDisplayLib.GridViewDisplayControl();
            this.EntityListSourceTabs = new System.Windows.Forms.TabControl();
            this.QueryTabPage = new System.Windows.Forms.TabPage();
            this.JobListPanel1 = new MageUIComponents.JobListPanel();
            this.JobsFlexQueryTabPage = new System.Windows.Forms.TabPage();
            this.JobFlexQueryPanel = new MageUIComponents.FlexQueryPanel();
            this.JobListTabPage = new System.Windows.Forms.TabPage();
            this.JobIDListPanel1 = new MageUIComponents.JobIDListPanel();
            this.JobsFromDatasetIDTabPage = new System.Windows.Forms.TabPage();
            this.JobDatasetIDList1 = new MageUIComponents.JobIDListPanel();
            this.DataPackageJobsTabPage = new System.Windows.Forms.TabPage();
            this.JobDataPackagePanel1 = new MageUIComponents.JobDataPackagePanel();
            this.DataPackageDatasetsTabPage = new System.Windows.Forms.TabPage();
            this.JobDataPackagePanel2 = new MageUIComponents.JobDataPackagePanel();
            this.DatasetTabPage = new System.Windows.Forms.TabPage();
            this.DatasetQueryPanel1 = new MageUIComponents.DatasetQueryPanel();
            this.DatasetIDTabPage = new System.Windows.Forms.TabPage();
            this.DatasetIDListPanel1 = new MageUIComponents.DatasetIDListPanel();
            this.AboutTabPage = new System.Windows.Forms.TabPage();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.lblAboutLink = new System.Windows.Forms.LinkLabel();
            this.txtAbout2 = new System.Windows.Forms.TextBox();
            this.txtAbout1 = new System.Windows.Forms.TextBox();
            this.txtAbout3 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.FileListDisplayControl = new MageDisplayLib.GridViewDisplayControl();
            this.FileSourceTabs = new System.Windows.Forms.TabControl();
            this.GetEntityFilesTabPage = new System.Windows.Forms.TabPage();
            this.EntityFilePanel1 = new MageUIComponents.EntityFilePanel();
            this.GetLocalFileTabPage = new System.Windows.Forms.TabPage();
            this.LocalFolderPanel1 = new MageUIComponents.LocalFolderPanel();
            this.ManifestFileTabPage = new System.Windows.Forms.TabPage();
            this.LocalManifestPanel1 = new MageUIComponents.LocalManifestPanel();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.FilterOutputTabs.SuspendLayout();
            this.CopyFilesTabPage.SuspendLayout();
            this.ProcessFilesToLocalTabPage.SuspendLayout();
            this.ProcessFileToSQLiteDBTabPage.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.EntityListSourceTabs.SuspendLayout();
            this.QueryTabPage.SuspendLayout();
            this.JobsFlexQueryTabPage.SuspendLayout();
            this.JobListTabPage.SuspendLayout();
            this.JobsFromDatasetIDTabPage.SuspendLayout();
            this.DataPackageJobsTabPage.SuspendLayout();
            this.DataPackageDatasetsTabPage.SuspendLayout();
            this.DatasetTabPage.SuspendLayout();
            this.DatasetIDTabPage.SuspendLayout();
            this.AboutTabPage.SuspendLayout();
            this.pnlAbout.SuspendLayout();
            this.panel2.SuspendLayout();
            this.FileSourceTabs.SuspendLayout();
            this.GetEntityFilesTabPage.SuspendLayout();
            this.GetLocalFileTabPage.SuspendLayout();
            this.ManifestFileTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.FileProcessingPanel1);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 595);
            this.panel3.Name = "panel3";
            this.panel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panel3.Size = new System.Drawing.Size(1108, 243);
            this.panel3.TabIndex = 6;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.statusPanel1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 196);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1108, 47);
            this.panel5.TabIndex = 17;
            // 
            // statusPanel1
            // 
            this.statusPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.statusPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusPanel1.EnableCancel = true;
            this.statusPanel1.Location = new System.Drawing.Point(0, 0);
            this.statusPanel1.Name = "statusPanel1";
            this.statusPanel1.OwnerControl = this;
            this.statusPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.statusPanel1.ShowCancel = true;
            this.statusPanel1.Size = new System.Drawing.Size(1108, 47);
            this.statusPanel1.TabIndex = 3;
            // 
            // FileProcessingPanel1
            // 
            this.FileProcessingPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileProcessingPanel1.Location = new System.Drawing.Point(0, 110);
            this.FileProcessingPanel1.Name = "FileProcessingPanel1";
            this.FileProcessingPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.FileProcessingPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FileProcessingPanel1.Size = new System.Drawing.Size(1108, 133);
            this.FileProcessingPanel1.TabIndex = 12;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.FilterOutputTabs);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5);
            this.panel4.Size = new System.Drawing.Size(1108, 110);
            this.panel4.TabIndex = 16;
            // 
            // FilterOutputTabs
            // 
            this.FilterOutputTabs.Controls.Add(this.CopyFilesTabPage);
            this.FilterOutputTabs.Controls.Add(this.ProcessFilesToLocalTabPage);
            this.FilterOutputTabs.Controls.Add(this.ProcessFileToSQLiteDBTabPage);
            this.FilterOutputTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterOutputTabs.Location = new System.Drawing.Point(5, 5);
            this.FilterOutputTabs.Name = "FilterOutputTabs";
            this.FilterOutputTabs.SelectedIndex = 0;
            this.FilterOutputTabs.Size = new System.Drawing.Size(1098, 100);
            this.FilterOutputTabs.TabIndex = 15;
            this.FilterOutputTabs.Selected += new System.Windows.Forms.TabControlEventHandler(this.FilterOutputTabs_Selected);
            // 
            // CopyFilesTabPage
            // 
            this.CopyFilesTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.CopyFilesTabPage.Controls.Add(this.FileCopyPanel1);
            this.CopyFilesTabPage.Location = new System.Drawing.Point(4, 22);
            this.CopyFilesTabPage.Name = "CopyFilesTabPage";
            this.CopyFilesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.CopyFilesTabPage.Size = new System.Drawing.Size(1090, 74);
            this.CopyFilesTabPage.TabIndex = 2;
            this.CopyFilesTabPage.Tag = "Copy_Files";
            this.CopyFilesTabPage.Text = "Copy Files To Local Folder";
            // 
            // FileCopyPanel1
            // 
            this.FileCopyPanel1.ApplyPrefixToFileName = "Yes";
            this.FileCopyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileCopyPanel1.Location = new System.Drawing.Point(3, 3);
            this.FileCopyPanel1.Name = "FileCopyPanel1";
            this.FileCopyPanel1.OutputFolder = "C:\\Data\\Junk";
            this.FileCopyPanel1.OverwriteExistingFiles = "No";
            this.FileCopyPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.FileCopyPanel1.PrefixColumnName = "";
            this.FileCopyPanel1.PrefixLeader = "";
            this.FileCopyPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FileCopyPanel1.Size = new System.Drawing.Size(1084, 68);
            this.FileCopyPanel1.TabIndex = 0;
            // 
            // ProcessFilesToLocalTabPage
            // 
            this.ProcessFilesToLocalTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.ProcessFilesToLocalTabPage.Controls.Add(this.FolderDestinationPanel1);
            this.ProcessFilesToLocalTabPage.Location = new System.Drawing.Point(4, 22);
            this.ProcessFilesToLocalTabPage.Name = "ProcessFilesToLocalTabPage";
            this.ProcessFilesToLocalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ProcessFilesToLocalTabPage.Size = new System.Drawing.Size(1090, 74);
            this.ProcessFilesToLocalTabPage.TabIndex = 0;
            this.ProcessFilesToLocalTabPage.Tag = "File_Output";
            this.ProcessFilesToLocalTabPage.Text = "Process Files To Local Folder";
            // 
            // FolderDestinationPanel1
            // 
            this.FolderDestinationPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.FolderDestinationPanel1.Location = new System.Drawing.Point(3, 3);
            this.FolderDestinationPanel1.Name = "FolderDestinationPanel1";
            this.FolderDestinationPanel1.OutputFile = "";
            this.FolderDestinationPanel1.OutputFolder = "C:\\Data\\Junk";
            this.FolderDestinationPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.FolderDestinationPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FolderDestinationPanel1.Size = new System.Drawing.Size(1084, 68);
            this.FolderDestinationPanel1.TabIndex = 13;
            // 
            // ProcessFileToSQLiteDBTabPage
            // 
            this.ProcessFileToSQLiteDBTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.ProcessFileToSQLiteDBTabPage.Controls.Add(this.SQLiteDestinationPanel1);
            this.ProcessFileToSQLiteDBTabPage.Location = new System.Drawing.Point(4, 22);
            this.ProcessFileToSQLiteDBTabPage.Name = "ProcessFileToSQLiteDBTabPage";
            this.ProcessFileToSQLiteDBTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ProcessFileToSQLiteDBTabPage.Size = new System.Drawing.Size(1090, 74);
            this.ProcessFileToSQLiteDBTabPage.TabIndex = 1;
            this.ProcessFileToSQLiteDBTabPage.Tag = "SQLite_Output";
            this.ProcessFileToSQLiteDBTabPage.Text = "Process Files To SQLite Database";
            // 
            // SQLiteDestinationPanel1
            // 
            this.SQLiteDestinationPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.SQLiteDestinationPanel1.DatabaseName = "C:\\Data\\test.db";
            this.SQLiteDestinationPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SQLiteDestinationPanel1.Location = new System.Drawing.Point(3, 3);
            this.SQLiteDestinationPanel1.Name = "SQLiteDestinationPanel1";
            this.SQLiteDestinationPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SQLiteDestinationPanel1.Size = new System.Drawing.Size(1084, 68);
            this.SQLiteDestinationPanel1.TabIndex = 14;
            this.SQLiteDestinationPanel1.TableName = "DMS_Factors";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 595);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(1105, 595);
            this.splitContainer1.SplitterDistance = 310;
            this.splitContainer1.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.JobListDisplayControl);
            this.panel1.Controls.Add(this.EntityListSourceTabs);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(1105, 310);
            this.panel1.TabIndex = 4;
            // 
            // JobListDisplayControl
            // 
            this.JobListDisplayControl.AllowDisableShiftClickMode = true;
            this.JobListDisplayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobListDisplayControl.HeaderVisible = true;
            this.JobListDisplayControl.ItemBlockSize = 100;
            this.JobListDisplayControl.Location = new System.Drawing.Point(5, 145);
            this.JobListDisplayControl.MultiSelect = true;
            this.JobListDisplayControl.Name = "JobListDisplayControl";
            this.JobListDisplayControl.Notice = "";
            this.JobListDisplayControl.PageTitle = "Title";
            this.JobListDisplayControl.Size = new System.Drawing.Size(1093, 158);
            this.JobListDisplayControl.TabIndex = 4;
            // 
            // EntityListSourceTabs
            // 
            this.EntityListSourceTabs.AccessibleDescription = " ";
            this.EntityListSourceTabs.Controls.Add(this.QueryTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobsFlexQueryTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobListTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobsFromDatasetIDTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DataPackageJobsTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DataPackageDatasetsTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DatasetTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DatasetIDTabPage);
            this.EntityListSourceTabs.Controls.Add(this.AboutTabPage);
            this.EntityListSourceTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.EntityListSourceTabs.Location = new System.Drawing.Point(5, 5);
            this.EntityListSourceTabs.Name = "EntityListSourceTabs";
            this.EntityListSourceTabs.SelectedIndex = 0;
            this.EntityListSourceTabs.Size = new System.Drawing.Size(1093, 140);
            this.EntityListSourceTabs.TabIndex = 3;
            this.EntityListSourceTabs.Tag = "Job_List";
            // 
            // QueryTabPage
            // 
            this.QueryTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.QueryTabPage.Controls.Add(this.JobListPanel1);
            this.QueryTabPage.Location = new System.Drawing.Point(4, 22);
            this.QueryTabPage.Name = "QueryTabPage";
            this.QueryTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.QueryTabPage.Size = new System.Drawing.Size(1085, 114);
            this.QueryTabPage.TabIndex = 0;
            this.QueryTabPage.Tag = "Jobs";
            this.QueryTabPage.Text = "Jobs From Query";
            // 
            // JobListPanel1
            // 
            this.JobListPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.JobListPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobListPanel1.Location = new System.Drawing.Point(3, 3);
            this.JobListPanel1.Name = "JobListPanel1";
            this.JobListPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.JobListPanel1.Size = new System.Drawing.Size(1079, 108);
            this.JobListPanel1.TabIndex = 0;
            // 
            // JobsFlexQueryTabPage
            // 
            this.JobsFlexQueryTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.JobsFlexQueryTabPage.Controls.Add(this.JobFlexQueryPanel);
            this.JobsFlexQueryTabPage.Location = new System.Drawing.Point(4, 22);
            this.JobsFlexQueryTabPage.Name = "JobsFlexQueryTabPage";
            this.JobsFlexQueryTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.JobsFlexQueryTabPage.Size = new System.Drawing.Size(1085, 114);
            this.JobsFlexQueryTabPage.TabIndex = 6;
            this.JobsFlexQueryTabPage.Tag = "Job_Flex_Query";
            this.JobsFlexQueryTabPage.Text = "Jobs From Flex Query";
            // 
            // JobFlexQueryPanel
            // 
            this.JobFlexQueryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobFlexQueryPanel.Location = new System.Drawing.Point(3, 3);
            this.JobFlexQueryPanel.Name = "JobFlexQueryPanel";
            this.JobFlexQueryPanel.Padding = new System.Windows.Forms.Padding(5);
            this.JobFlexQueryPanel.QueryName = null;
            this.JobFlexQueryPanel.Size = new System.Drawing.Size(1079, 108);
            this.JobFlexQueryPanel.TabIndex = 0;
            // 
            // JobListTabPage
            // 
            this.JobListTabPage.BackColor = System.Drawing.Color.Transparent;
            this.JobListTabPage.Controls.Add(this.JobIDListPanel1);
            this.JobListTabPage.Location = new System.Drawing.Point(4, 22);
            this.JobListTabPage.Name = "JobListTabPage";
            this.JobListTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.JobListTabPage.Size = new System.Drawing.Size(1085, 114);
            this.JobListTabPage.TabIndex = 1;
            this.JobListTabPage.Text = "Jobs From Job List";
            this.JobListTabPage.UseVisualStyleBackColor = true;
            // 
            // JobIDListPanel1
            // 
            this.JobIDListPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.JobIDListPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.JobIDListPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobIDListPanel1.Legend = "(Paste in list of job IDs)";
            this.JobIDListPanel1.ListName = "Job";
            this.JobIDListPanel1.Location = new System.Drawing.Point(3, 3);
            this.JobIDListPanel1.Name = "JobIDListPanel1";
            this.JobIDListPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.JobIDListPanel1.Size = new System.Drawing.Size(1079, 108);
            this.JobIDListPanel1.TabIndex = 0;
            // 
            // JobsFromDatasetIDTabPage
            // 
            this.JobsFromDatasetIDTabPage.BackColor = System.Drawing.Color.Transparent;
            this.JobsFromDatasetIDTabPage.Controls.Add(this.JobDatasetIDList1);
            this.JobsFromDatasetIDTabPage.Location = new System.Drawing.Point(4, 22);
            this.JobsFromDatasetIDTabPage.Name = "JobsFromDatasetIDTabPage";
            this.JobsFromDatasetIDTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.JobsFromDatasetIDTabPage.Size = new System.Drawing.Size(1085, 114);
            this.JobsFromDatasetIDTabPage.TabIndex = 5;
            this.JobsFromDatasetIDTabPage.Text = "Jobs From Dataset List";
            this.JobsFromDatasetIDTabPage.UseVisualStyleBackColor = true;
            // 
            // JobDatasetIDList1
            // 
            this.JobDatasetIDList1.BackColor = System.Drawing.SystemColors.Control;
            this.JobDatasetIDList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDatasetIDList1.Legend = "(Paste in list of job IDs)";
            this.JobDatasetIDList1.ListName = "Job";
            this.JobDatasetIDList1.Location = new System.Drawing.Point(3, 3);
            this.JobDatasetIDList1.Name = "JobDatasetIDList1";
            this.JobDatasetIDList1.Padding = new System.Windows.Forms.Padding(5);
            this.JobDatasetIDList1.Size = new System.Drawing.Size(1079, 108);
            this.JobDatasetIDList1.TabIndex = 0;
            // 
            // DataPackageJobsTabPage
            // 
            this.DataPackageJobsTabPage.BackColor = System.Drawing.Color.Transparent;
            this.DataPackageJobsTabPage.Controls.Add(this.JobDataPackagePanel1);
            this.DataPackageJobsTabPage.Location = new System.Drawing.Point(4, 22);
            this.DataPackageJobsTabPage.Name = "DataPackageJobsTabPage";
            this.DataPackageJobsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DataPackageJobsTabPage.Size = new System.Drawing.Size(1085, 114);
            this.DataPackageJobsTabPage.TabIndex = 2;
            this.DataPackageJobsTabPage.Tag = "Data_Package";
            this.DataPackageJobsTabPage.Text = "Jobs From Data Pkg";
            this.DataPackageJobsTabPage.UseVisualStyleBackColor = true;
            // 
            // JobDataPackagePanel1
            // 
            this.JobDataPackagePanel1.BackColor = System.Drawing.SystemColors.Control;
            this.JobDataPackagePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDataPackagePanel1.Location = new System.Drawing.Point(3, 3);
            this.JobDataPackagePanel1.Name = "JobDataPackagePanel1";
            this.JobDataPackagePanel1.Padding = new System.Windows.Forms.Padding(5);
            this.JobDataPackagePanel1.ShowGetDatasets = false;
            this.JobDataPackagePanel1.ShowGetJobs = true;
            this.JobDataPackagePanel1.Size = new System.Drawing.Size(1079, 108);
            this.JobDataPackagePanel1.TabIndex = 0;
            // 
            // DataPackageDatasetsTabPage
            // 
            this.DataPackageDatasetsTabPage.Controls.Add(this.JobDataPackagePanel2);
            this.DataPackageDatasetsTabPage.Location = new System.Drawing.Point(4, 22);
            this.DataPackageDatasetsTabPage.Name = "DataPackageDatasetsTabPage";
            this.DataPackageDatasetsTabPage.Size = new System.Drawing.Size(1085, 114);
            this.DataPackageDatasetsTabPage.TabIndex = 8;
            this.DataPackageDatasetsTabPage.Tag = "Data_Package_Datasets";
            this.DataPackageDatasetsTabPage.Text = "Datasets From Data Pkg";
            this.DataPackageDatasetsTabPage.UseVisualStyleBackColor = true;
            // 
            // JobDataPackagePanel2
            // 
            this.JobDataPackagePanel2.BackColor = System.Drawing.SystemColors.Control;
            this.JobDataPackagePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDataPackagePanel2.Location = new System.Drawing.Point(0, 0);
            this.JobDataPackagePanel2.Name = "JobDataPackagePanel2";
            this.JobDataPackagePanel2.Padding = new System.Windows.Forms.Padding(5);
            this.JobDataPackagePanel2.ShowGetDatasets = true;
            this.JobDataPackagePanel2.ShowGetJobs = false;
            this.JobDataPackagePanel2.Size = new System.Drawing.Size(1085, 114);
            this.JobDataPackagePanel2.TabIndex = 1;
            // 
            // DatasetTabPage
            // 
            this.DatasetTabPage.BackColor = System.Drawing.Color.Transparent;
            this.DatasetTabPage.Controls.Add(this.DatasetQueryPanel1);
            this.DatasetTabPage.Location = new System.Drawing.Point(4, 22);
            this.DatasetTabPage.Name = "DatasetTabPage";
            this.DatasetTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DatasetTabPage.Size = new System.Drawing.Size(1085, 114);
            this.DatasetTabPage.TabIndex = 3;
            this.DatasetTabPage.Text = "Datasets From Query";
            this.DatasetTabPage.UseVisualStyleBackColor = true;
            // 
            // DatasetQueryPanel1
            // 
            this.DatasetQueryPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.DatasetQueryPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatasetQueryPanel1.Location = new System.Drawing.Point(3, 3);
            this.DatasetQueryPanel1.Name = "DatasetQueryPanel1";
            this.DatasetQueryPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.DatasetQueryPanel1.Size = new System.Drawing.Size(1079, 108);
            this.DatasetQueryPanel1.TabIndex = 0;
            // 
            // DatasetIDTabPage
            // 
            this.DatasetIDTabPage.BackColor = System.Drawing.Color.Transparent;
            this.DatasetIDTabPage.Controls.Add(this.DatasetIDListPanel1);
            this.DatasetIDTabPage.Location = new System.Drawing.Point(4, 22);
            this.DatasetIDTabPage.Name = "DatasetIDTabPage";
            this.DatasetIDTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.DatasetIDTabPage.Size = new System.Drawing.Size(1085, 114);
            this.DatasetIDTabPage.TabIndex = 4;
            this.DatasetIDTabPage.Text = "Datasets From Dataset List";
            this.DatasetIDTabPage.UseVisualStyleBackColor = true;
            // 
            // DatasetIDListPanel1
            // 
            this.DatasetIDListPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.DatasetIDListPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatasetIDListPanel1.Location = new System.Drawing.Point(3, 3);
            this.DatasetIDListPanel1.Name = "DatasetIDListPanel1";
            this.DatasetIDListPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.DatasetIDListPanel1.Size = new System.Drawing.Size(1079, 108);
            this.DatasetIDListPanel1.TabIndex = 0;
            // 
            // AboutTabPage
            // 
            this.AboutTabPage.Controls.Add(this.pnlAbout);
            this.AboutTabPage.Location = new System.Drawing.Point(4, 22);
            this.AboutTabPage.Name = "AboutTabPage";
            this.AboutTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.AboutTabPage.Size = new System.Drawing.Size(1085, 114);
            this.AboutTabPage.TabIndex = 7;
            this.AboutTabPage.Text = "About";
            this.AboutTabPage.UseVisualStyleBackColor = true;
            // 
            // pnlAbout
            // 
            this.pnlAbout.BackColor = System.Drawing.SystemColors.Control;
            this.pnlAbout.Controls.Add(this.txtVersion);
            this.pnlAbout.Controls.Add(this.lblAboutLink);
            this.pnlAbout.Controls.Add(this.txtAbout2);
            this.pnlAbout.Controls.Add(this.txtAbout1);
            this.pnlAbout.Controls.Add(this.txtAbout3);
            this.pnlAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAbout.Location = new System.Drawing.Point(3, 3);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Size = new System.Drawing.Size(1079, 108);
            this.pnlAbout.TabIndex = 1;
            // 
            // txtVersion
            // 
            this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtVersion.Location = new System.Drawing.Point(776, 77);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(300, 13);
            this.txtVersion.TabIndex = 5;
            this.txtVersion.Text = "Version";
            // 
            // lblAboutLink
            // 
            this.lblAboutLink.AutoSize = true;
            this.lblAboutLink.Location = new System.Drawing.Point(201, 77);
            this.lblAboutLink.Name = "lblAboutLink";
            this.lblAboutLink.Size = new System.Drawing.Size(65, 13);
            this.lblAboutLink.TabIndex = 3;
            this.lblAboutLink.TabStop = true;
            this.lblAboutLink.Text = "lblAboutLink";
            // 
            // txtAbout2
            // 
            this.txtAbout2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAbout2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout2.Location = new System.Drawing.Point(12, 50);
            this.txtAbout2.Name = "txtAbout2";
            this.txtAbout2.ReadOnly = true;
            this.txtAbout2.Size = new System.Drawing.Size(1053, 13);
            this.txtAbout2.TabIndex = 2;
            this.txtAbout2.Text = "Written by ...";
            // 
            // txtAbout1
            // 
            this.txtAbout1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAbout1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout1.Location = new System.Drawing.Point(12, 10);
            this.txtAbout1.Multiline = true;
            this.txtAbout1.Name = "txtAbout1";
            this.txtAbout1.ReadOnly = true;
            this.txtAbout1.Size = new System.Drawing.Size(1053, 36);
            this.txtAbout1.TabIndex = 0;
            this.txtAbout1.Text = "Mage File Processor can ...";
            // 
            // txtAbout3
            // 
            this.txtAbout3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout3.Location = new System.Drawing.Point(12, 77);
            this.txtAbout3.Name = "txtAbout3";
            this.txtAbout3.ReadOnly = true;
            this.txtAbout3.Size = new System.Drawing.Size(318, 13);
            this.txtAbout3.TabIndex = 4;
            this.txtAbout3.Text = "For usage instructions, please see";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.FileListDisplayControl);
            this.panel2.Controls.Add(this.FileSourceTabs);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(1105, 281);
            this.panel2.TabIndex = 5;
            // 
            // FileListDisplayControl
            // 
            this.FileListDisplayControl.AllowDisableShiftClickMode = true;
            this.FileListDisplayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileListDisplayControl.HeaderVisible = true;
            this.FileListDisplayControl.ItemBlockSize = 100;
            this.FileListDisplayControl.Location = new System.Drawing.Point(5, 111);
            this.FileListDisplayControl.MultiSelect = true;
            this.FileListDisplayControl.Name = "FileListDisplayControl";
            this.FileListDisplayControl.Notice = "";
            this.FileListDisplayControl.PageTitle = "Title";
            this.FileListDisplayControl.Size = new System.Drawing.Size(1093, 163);
            this.FileListDisplayControl.TabIndex = 12;
            // 
            // FileSourceTabs
            // 
            this.FileSourceTabs.Controls.Add(this.GetEntityFilesTabPage);
            this.FileSourceTabs.Controls.Add(this.GetLocalFileTabPage);
            this.FileSourceTabs.Controls.Add(this.ManifestFileTabPage);
            this.FileSourceTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.FileSourceTabs.Location = new System.Drawing.Point(5, 5);
            this.FileSourceTabs.Name = "FileSourceTabs";
            this.FileSourceTabs.SelectedIndex = 0;
            this.FileSourceTabs.Size = new System.Drawing.Size(1093, 106);
            this.FileSourceTabs.TabIndex = 11;
            // 
            // GetEntityFilesTabPage
            // 
            this.GetEntityFilesTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.GetEntityFilesTabPage.Controls.Add(this.EntityFilePanel1);
            this.GetEntityFilesTabPage.Location = new System.Drawing.Point(4, 22);
            this.GetEntityFilesTabPage.Name = "GetEntityFilesTabPage";
            this.GetEntityFilesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.GetEntityFilesTabPage.Size = new System.Drawing.Size(1085, 80);
            this.GetEntityFilesTabPage.TabIndex = 0;
            this.GetEntityFilesTabPage.Tag = "Job_Files";
            this.GetEntityFilesTabPage.Text = "Get Entity Files";
            // 
            // EntityFilePanel1
            // 
            this.EntityFilePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EntityFilePanel1.FileSelectionMode = "RegEx";
            this.EntityFilePanel1.FileSelectors = "log";
            this.EntityFilePanel1.IncludeFilesOrFolders = "File";
            this.EntityFilePanel1.Location = new System.Drawing.Point(3, 3);
            this.EntityFilePanel1.Name = "EntityFilePanel1";
            this.EntityFilePanel1.Padding = new System.Windows.Forms.Padding(5);
            this.EntityFilePanel1.SearchInSubfolders = "No";
            this.EntityFilePanel1.Size = new System.Drawing.Size(1079, 74);
            this.EntityFilePanel1.SubfolderSearchName = "*";
            this.EntityFilePanel1.TabIndex = 10;
            // 
            // GetLocalFileTabPage
            // 
            this.GetLocalFileTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.GetLocalFileTabPage.Controls.Add(this.LocalFolderPanel1);
            this.GetLocalFileTabPage.Location = new System.Drawing.Point(4, 22);
            this.GetLocalFileTabPage.Name = "GetLocalFileTabPage";
            this.GetLocalFileTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.GetLocalFileTabPage.Size = new System.Drawing.Size(1085, 80);
            this.GetLocalFileTabPage.TabIndex = 1;
            this.GetLocalFileTabPage.Tag = "Local_Files";
            this.GetLocalFileTabPage.Text = "Get Local Files";
            // 
            // LocalFolderPanel1
            // 
            this.LocalFolderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocalFolderPanel1.FileNameFilter = "syn.txt";
            this.LocalFolderPanel1.Folder = "C:\\Data\\syn";
            this.LocalFolderPanel1.Location = new System.Drawing.Point(3, 3);
            this.LocalFolderPanel1.Name = "LocalFolderPanel1";
            this.LocalFolderPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.LocalFolderPanel1.SearchInSubfolders = "No";
            this.LocalFolderPanel1.Size = new System.Drawing.Size(1079, 74);
            this.LocalFolderPanel1.SubfolderSearchName = "*";
            this.LocalFolderPanel1.TabIndex = 7;
            // 
            // ManifestFileTabPage
            // 
            this.ManifestFileTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.ManifestFileTabPage.Controls.Add(this.LocalManifestPanel1);
            this.ManifestFileTabPage.Location = new System.Drawing.Point(4, 22);
            this.ManifestFileTabPage.Name = "ManifestFileTabPage";
            this.ManifestFileTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ManifestFileTabPage.Size = new System.Drawing.Size(1085, 80);
            this.ManifestFileTabPage.TabIndex = 2;
            this.ManifestFileTabPage.Tag = "Manifest_Files";
            this.ManifestFileTabPage.Text = "Get Manifest Files";
            // 
            // LocalManifestPanel1
            // 
            this.LocalManifestPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocalManifestPanel1.Location = new System.Drawing.Point(3, 3);
            this.LocalManifestPanel1.ManifestFilePath = "C:\\Data\\syn\\Manifest_101112020324.txt";
            this.LocalManifestPanel1.Name = "LocalManifestPanel1";
            this.LocalManifestPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.LocalManifestPanel1.Size = new System.Drawing.Size(1079, 74);
            this.LocalManifestPanel1.TabIndex = 0;
            // 
            // FileProcessorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1108, 838);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(750, 750);
            this.Name = "FileProcessorForm";
            this.Text = "Mage File Processor";
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.FilterOutputTabs.ResumeLayout(false);
            this.CopyFilesTabPage.ResumeLayout(false);
            this.ProcessFilesToLocalTabPage.ResumeLayout(false);
            this.ProcessFileToSQLiteDBTabPage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.EntityListSourceTabs.ResumeLayout(false);
            this.QueryTabPage.ResumeLayout(false);
            this.JobsFlexQueryTabPage.ResumeLayout(false);
            this.JobListTabPage.ResumeLayout(false);
            this.JobsFromDatasetIDTabPage.ResumeLayout(false);
            this.DataPackageJobsTabPage.ResumeLayout(false);
            this.DataPackageDatasetsTabPage.ResumeLayout(false);
            this.DatasetTabPage.ResumeLayout(false);
            this.DatasetIDTabPage.ResumeLayout(false);
            this.AboutTabPage.ResumeLayout(false);
            this.pnlAbout.ResumeLayout(false);
            this.pnlAbout.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.FileSourceTabs.ResumeLayout(false);
            this.GetEntityFilesTabPage.ResumeLayout(false);
            this.GetLocalFileTabPage.ResumeLayout(false);
            this.ManifestFileTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl EntityListSourceTabs;
        private System.Windows.Forms.TabPage QueryTabPage;
        private MageUIComponents.JobListPanel JobListPanel1;
        private System.Windows.Forms.TabPage JobListTabPage;
        private MageUIComponents.JobIDListPanel JobIDListPanel1;
        private System.Windows.Forms.TabPage DataPackageJobsTabPage;
        private MageUIComponents.JobDataPackagePanel JobDataPackagePanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
        private MageUIComponents.LocalFolderPanel LocalFolderPanel1;
        private MageUIComponents.EntityFilePanel EntityFilePanel1;
        private System.Windows.Forms.TabControl FileSourceTabs;
        private System.Windows.Forms.TabPage GetEntityFilesTabPage;
        private System.Windows.Forms.TabPage GetLocalFileTabPage;
        private MageUIComponents.FileProcessingPanel FileProcessingPanel1;
        private MageUIComponents.FolderDestinationPanel FolderDestinationPanel1;
        private MageUIComponents.SQLiteDestinationPanel SQLiteDestinationPanel1;
        private System.Windows.Forms.TabControl FilterOutputTabs;
        private System.Windows.Forms.TabPage ProcessFilesToLocalTabPage;
        private System.Windows.Forms.TabPage ProcessFileToSQLiteDBTabPage;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage DatasetTabPage;
        private MageUIComponents.DatasetQueryPanel DatasetQueryPanel1;
        private System.Windows.Forms.TabPage CopyFilesTabPage;
        private MageUIComponents.FileCopyPanel FileCopyPanel1;
        private System.Windows.Forms.TabPage DatasetIDTabPage;
        private MageUIComponents.DatasetIDListPanel DatasetIDListPanel1;
        private System.Windows.Forms.TabPage ManifestFileTabPage;
        private MageUIComponents.LocalManifestPanel LocalManifestPanel1;
        private System.Windows.Forms.TabPage JobsFromDatasetIDTabPage;
        private MageUIComponents.JobIDListPanel JobDatasetIDList1;
        private MageDisplayLib.GridViewDisplayControl JobListDisplayControl;
        private MageDisplayLib.GridViewDisplayControl FileListDisplayControl;
        private System.Windows.Forms.TabPage JobsFlexQueryTabPage;
		private MageUIComponents.FlexQueryPanel JobFlexQueryPanel;
		private System.Windows.Forms.Panel panel5;
		private MageDisplayLib.StatusPanel statusPanel1;
		private System.Windows.Forms.TabPage AboutTabPage;
		private System.Windows.Forms.Panel pnlAbout;
		private System.Windows.Forms.TextBox txtAbout3;
		private System.Windows.Forms.LinkLabel lblAboutLink;
		private System.Windows.Forms.TextBox txtAbout2;
		private System.Windows.Forms.TextBox txtAbout1;
		private System.Windows.Forms.TextBox txtVersion;
		private System.Windows.Forms.TabPage DataPackageDatasetsTabPage;
		private MageUIComponents.JobDataPackagePanel JobDataPackagePanel2;
    }
}

