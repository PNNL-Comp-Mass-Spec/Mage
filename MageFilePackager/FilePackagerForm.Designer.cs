namespace MageFilePackager {
    partial class FilePackagerForm {
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.JobListDisplayControl = new MageDisplayLib.GridViewDisplayControl();
            this.EntityListSourceTabs = new System.Windows.Forms.TabControl();
            this.DataPackageDetailsTabPage = new System.Windows.Forms.TabPage();
            this.dataPackageDetailsListPanel1 = new MageUIComponents.DataPackageDetailsListPanel();
            this.DataPackageDatasetsTabPage = new System.Windows.Forms.TabPage();
            this.JobDataPackagePanel2 = new MageUIComponents.JobDataPackagePanel();
            this.DataPackageJobsTabPage = new System.Windows.Forms.TabPage();
            this.JobDataPackagePanel1 = new MageUIComponents.JobDataPackagePanel();
            this.DatasetTabPage = new System.Windows.Forms.TabPage();
            this.DatasetQueryPanel1 = new MageUIComponents.DatasetQueryPanel();
            this.DatasetIDTabPage = new System.Windows.Forms.TabPage();
            this.DatasetIDListPanel1 = new MageUIComponents.DatasetIDListPanel();
            this.QueryTabPage = new System.Windows.Forms.TabPage();
            this.JobListPanel1 = new MageUIComponents.JobListPanel();
            this.JobsFlexQueryTabPage = new System.Windows.Forms.TabPage();
            this.JobFlexQueryPanel = new MageUIComponents.FlexQueryPanel();
            this.JobListTabPage = new System.Windows.Forms.TabPage();
            this.JobIDListPanel1 = new MageUIComponents.JobIDListPanel();
            this.JobsFromDatasetIDTabPage = new System.Windows.Forms.TabPage();
            this.JobDatasetIDList1 = new MageUIComponents.JobIDListPanel();
            this.AboutTabPage = new System.Windows.Forms.TabPage();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.lblAboutLink = new System.Windows.Forms.LinkLabel();
            this.txtAbout2 = new System.Windows.Forms.TextBox();
            this.txtAbout1 = new System.Windows.Forms.TextBox();
            this.txtAbout3 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.FileListDisplayControl = new MageDisplayLib.GridViewDisplayControl();
            this.FileSourceTabs = new System.Windows.Forms.TabControl();
            this.GetEntityFilesTabPage = new System.Windows.Forms.TabPage();
            this.EntityFilePanel1 = new MageUIComponents.EntityFilePanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.statusPanel1 = new MageDisplayLib.StatusPanel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.filePackageMgmtPanel1 = new MageFilePackager.FilePackageMgmtPanel();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.EntityListSourceTabs.SuspendLayout();
            this.DataPackageDetailsTabPage.SuspendLayout();
            this.DataPackageDatasetsTabPage.SuspendLayout();
            this.DataPackageJobsTabPage.SuspendLayout();
            this.DatasetTabPage.SuspendLayout();
            this.DatasetIDTabPage.SuspendLayout();
            this.QueryTabPage.SuspendLayout();
            this.JobsFlexQueryTabPage.SuspendLayout();
            this.JobListTabPage.SuspendLayout();
            this.JobsFromDatasetIDTabPage.SuspendLayout();
            this.AboutTabPage.SuspendLayout();
            this.pnlAbout.SuspendLayout();
            this.panel2.SuspendLayout();
            this.FileSourceTabs.SuspendLayout();
            this.GetEntityFilesTabPage.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.splitContainer1.Size = new System.Drawing.Size(1572, 647);
            this.splitContainer1.SplitterDistance = 334;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.JobListDisplayControl);
            this.panel1.Controls.Add(this.EntityListSourceTabs);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.panel1.Size = new System.Drawing.Size(1572, 334);
            this.panel1.TabIndex = 4;
            // 
            // JobListDisplayControl
            // 
            this.JobListDisplayControl.AllowDisableShiftClickMode = true;
            this.JobListDisplayControl.AutoSizeColumnWidths = false;
            this.JobListDisplayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobListDisplayControl.HeaderVisible = true;
            this.JobListDisplayControl.ItemBlockSize = 100;
            this.JobListDisplayControl.Location = new System.Drawing.Point(7, 178);
            this.JobListDisplayControl.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.JobListDisplayControl.MultiSelect = true;
            this.JobListDisplayControl.Name = "JobListDisplayControl";
            this.JobListDisplayControl.Notice = "";
            this.JobListDisplayControl.PageTitle = "Title";
            this.JobListDisplayControl.Size = new System.Drawing.Size(1556, 148);
            this.JobListDisplayControl.TabIndex = 4;
            // 
            // EntityListSourceTabs
            // 
            this.EntityListSourceTabs.AccessibleDescription = " ";
            this.EntityListSourceTabs.Controls.Add(this.DataPackageDetailsTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DataPackageDatasetsTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DataPackageJobsTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DatasetTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DatasetIDTabPage);
            this.EntityListSourceTabs.Controls.Add(this.QueryTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobsFlexQueryTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobListTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobsFromDatasetIDTabPage);
            this.EntityListSourceTabs.Controls.Add(this.AboutTabPage);
            this.EntityListSourceTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.EntityListSourceTabs.Location = new System.Drawing.Point(7, 6);
            this.EntityListSourceTabs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.EntityListSourceTabs.Name = "EntityListSourceTabs";
            this.EntityListSourceTabs.SelectedIndex = 0;
            this.EntityListSourceTabs.Size = new System.Drawing.Size(1556, 172);
            this.EntityListSourceTabs.TabIndex = 3;
            this.EntityListSourceTabs.Tag = "Job_List";
            // 
            // DataPackageDetailsTabPage
            // 
            this.DataPackageDetailsTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.DataPackageDetailsTabPage.Controls.Add(this.dataPackageDetailsListPanel1);
            this.DataPackageDetailsTabPage.Location = new System.Drawing.Point(4, 25);
            this.DataPackageDetailsTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DataPackageDetailsTabPage.Name = "DataPackageDetailsTabPage";
            this.DataPackageDetailsTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DataPackageDetailsTabPage.Size = new System.Drawing.Size(1548, 143);
            this.DataPackageDetailsTabPage.TabIndex = 9;
            this.DataPackageDetailsTabPage.Text = "Data Packages From List";
            // 
            // dataPackageDetailsListPanel1
            // 
            this.dataPackageDetailsListPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.dataPackageDetailsListPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPackageDetailsListPanel1.Location = new System.Drawing.Point(4, 4);
            this.dataPackageDetailsListPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.dataPackageDetailsListPanel1.Name = "dataPackageDetailsListPanel1";
            this.dataPackageDetailsListPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.dataPackageDetailsListPanel1.Size = new System.Drawing.Size(1540, 135);
            this.dataPackageDetailsListPanel1.TabIndex = 0;
            // 
            // DataPackageDatasetsTabPage
            // 
            this.DataPackageDatasetsTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.DataPackageDatasetsTabPage.Controls.Add(this.JobDataPackagePanel2);
            this.DataPackageDatasetsTabPage.Location = new System.Drawing.Point(4, 25);
            this.DataPackageDatasetsTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DataPackageDatasetsTabPage.Name = "DataPackageDatasetsTabPage";
            this.DataPackageDatasetsTabPage.Size = new System.Drawing.Size(1548, 143);
            this.DataPackageDatasetsTabPage.TabIndex = 8;
            this.DataPackageDatasetsTabPage.Tag = "Data_Package_Datasets";
            this.DataPackageDatasetsTabPage.Text = "Datasets From Data Pkg";
            // 
            // JobDataPackagePanel2
            // 
            this.JobDataPackagePanel2.BackColor = System.Drawing.SystemColors.Control;
            this.JobDataPackagePanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDataPackagePanel2.Location = new System.Drawing.Point(0, 0);
            this.JobDataPackagePanel2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.JobDataPackagePanel2.Name = "JobDataPackagePanel2";
            this.JobDataPackagePanel2.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobDataPackagePanel2.ShowGetDatasets = true;
            this.JobDataPackagePanel2.ShowGetJobs = false;
            this.JobDataPackagePanel2.Size = new System.Drawing.Size(1548, 143);
            this.JobDataPackagePanel2.TabIndex = 1;
            // 
            // DataPackageJobsTabPage
            // 
            this.DataPackageJobsTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.DataPackageJobsTabPage.Controls.Add(this.JobDataPackagePanel1);
            this.DataPackageJobsTabPage.Location = new System.Drawing.Point(4, 25);
            this.DataPackageJobsTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DataPackageJobsTabPage.Name = "DataPackageJobsTabPage";
            this.DataPackageJobsTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DataPackageJobsTabPage.Size = new System.Drawing.Size(1548, 143);
            this.DataPackageJobsTabPage.TabIndex = 2;
            this.DataPackageJobsTabPage.Tag = "Data_Package";
            this.DataPackageJobsTabPage.Text = "Jobs From Data Pkg";
            // 
            // JobDataPackagePanel1
            // 
            this.JobDataPackagePanel1.BackColor = System.Drawing.SystemColors.Control;
            this.JobDataPackagePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDataPackagePanel1.Location = new System.Drawing.Point(4, 4);
            this.JobDataPackagePanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.JobDataPackagePanel1.Name = "JobDataPackagePanel1";
            this.JobDataPackagePanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobDataPackagePanel1.ShowGetDatasets = false;
            this.JobDataPackagePanel1.ShowGetJobs = true;
            this.JobDataPackagePanel1.Size = new System.Drawing.Size(1540, 135);
            this.JobDataPackagePanel1.TabIndex = 0;
            // 
            // DatasetTabPage
            // 
            this.DatasetTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.DatasetTabPage.Controls.Add(this.DatasetQueryPanel1);
            this.DatasetTabPage.Location = new System.Drawing.Point(4, 25);
            this.DatasetTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatasetTabPage.Name = "DatasetTabPage";
            this.DatasetTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatasetTabPage.Size = new System.Drawing.Size(1548, 143);
            this.DatasetTabPage.TabIndex = 3;
            this.DatasetTabPage.Text = "Datasets From Query";
            // 
            // DatasetQueryPanel1
            // 
            this.DatasetQueryPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.DatasetQueryPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatasetQueryPanel1.Location = new System.Drawing.Point(4, 4);
            this.DatasetQueryPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.DatasetQueryPanel1.Name = "DatasetQueryPanel1";
            this.DatasetQueryPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.DatasetQueryPanel1.Size = new System.Drawing.Size(1540, 135);
            this.DatasetQueryPanel1.TabIndex = 0;
            // 
            // DatasetIDTabPage
            // 
            this.DatasetIDTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.DatasetIDTabPage.Controls.Add(this.DatasetIDListPanel1);
            this.DatasetIDTabPage.Location = new System.Drawing.Point(4, 25);
            this.DatasetIDTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatasetIDTabPage.Name = "DatasetIDTabPage";
            this.DatasetIDTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatasetIDTabPage.Size = new System.Drawing.Size(1548, 143);
            this.DatasetIDTabPage.TabIndex = 4;
            this.DatasetIDTabPage.Text = "Datasets From Dataset List";
            // 
            // DatasetIDListPanel1
            // 
            this.DatasetIDListPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.DatasetIDListPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatasetIDListPanel1.Location = new System.Drawing.Point(4, 4);
            this.DatasetIDListPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.DatasetIDListPanel1.Name = "DatasetIDListPanel1";
            this.DatasetIDListPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.DatasetIDListPanel1.Size = new System.Drawing.Size(1540, 135);
            this.DatasetIDListPanel1.TabIndex = 0;
            // 
            // QueryTabPage
            // 
            this.QueryTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.QueryTabPage.Controls.Add(this.JobListPanel1);
            this.QueryTabPage.Location = new System.Drawing.Point(4, 25);
            this.QueryTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.QueryTabPage.Name = "QueryTabPage";
            this.QueryTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.QueryTabPage.Size = new System.Drawing.Size(1548, 143);
            this.QueryTabPage.TabIndex = 0;
            this.QueryTabPage.Tag = "Jobs";
            this.QueryTabPage.Text = "Jobs From Query";
            // 
            // JobListPanel1
            // 
            this.JobListPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.JobListPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobListPanel1.Location = new System.Drawing.Point(4, 4);
            this.JobListPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.JobListPanel1.Name = "JobListPanel1";
            this.JobListPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobListPanel1.Size = new System.Drawing.Size(1540, 135);
            this.JobListPanel1.TabIndex = 0;
            // 
            // JobsFlexQueryTabPage
            // 
            this.JobsFlexQueryTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.JobsFlexQueryTabPage.Controls.Add(this.JobFlexQueryPanel);
            this.JobsFlexQueryTabPage.Location = new System.Drawing.Point(4, 25);
            this.JobsFlexQueryTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.JobsFlexQueryTabPage.Name = "JobsFlexQueryTabPage";
            this.JobsFlexQueryTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.JobsFlexQueryTabPage.Size = new System.Drawing.Size(1548, 143);
            this.JobsFlexQueryTabPage.TabIndex = 6;
            this.JobsFlexQueryTabPage.Tag = "Job_Flex_Query";
            this.JobsFlexQueryTabPage.Text = "Jobs From Flex Query";
            // 
            // JobFlexQueryPanel
            // 
            this.JobFlexQueryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobFlexQueryPanel.Location = new System.Drawing.Point(4, 4);
            this.JobFlexQueryPanel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.JobFlexQueryPanel.Name = "JobFlexQueryPanel";
            this.JobFlexQueryPanel.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobFlexQueryPanel.QueryName = null;
            this.JobFlexQueryPanel.Size = new System.Drawing.Size(1540, 135);
            this.JobFlexQueryPanel.TabIndex = 0;
            // 
            // JobListTabPage
            // 
            this.JobListTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.JobListTabPage.Controls.Add(this.JobIDListPanel1);
            this.JobListTabPage.Location = new System.Drawing.Point(4, 25);
            this.JobListTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.JobListTabPage.Name = "JobListTabPage";
            this.JobListTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.JobListTabPage.Size = new System.Drawing.Size(1548, 143);
            this.JobListTabPage.TabIndex = 1;
            this.JobListTabPage.Text = "Jobs From Job List";
            // 
            // JobIDListPanel1
            // 
            this.JobIDListPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.JobIDListPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.JobIDListPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobIDListPanel1.Legend = "(Job IDs)";
            this.JobIDListPanel1.ListName = "Job";
            this.JobIDListPanel1.Location = new System.Drawing.Point(4, 4);
            this.JobIDListPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.JobIDListPanel1.Name = "JobIDListPanel1";
            this.JobIDListPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobIDListPanel1.Size = new System.Drawing.Size(1540, 135);
            this.JobIDListPanel1.TabIndex = 0;
            // 
            // JobsFromDatasetIDTabPage
            // 
            this.JobsFromDatasetIDTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.JobsFromDatasetIDTabPage.Controls.Add(this.JobDatasetIDList1);
            this.JobsFromDatasetIDTabPage.Location = new System.Drawing.Point(4, 25);
            this.JobsFromDatasetIDTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.JobsFromDatasetIDTabPage.Name = "JobsFromDatasetIDTabPage";
            this.JobsFromDatasetIDTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.JobsFromDatasetIDTabPage.Size = new System.Drawing.Size(1548, 143);
            this.JobsFromDatasetIDTabPage.TabIndex = 5;
            this.JobsFromDatasetIDTabPage.Text = "Jobs From Dataset List";
            // 
            // JobDatasetIDList1
            // 
            this.JobDatasetIDList1.BackColor = System.Drawing.SystemColors.Control;
            this.JobDatasetIDList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDatasetIDList1.Legend = "(Job IDs)";
            this.JobDatasetIDList1.ListName = "Job";
            this.JobDatasetIDList1.Location = new System.Drawing.Point(4, 4);
            this.JobDatasetIDList1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.JobDatasetIDList1.Name = "JobDatasetIDList1";
            this.JobDatasetIDList1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobDatasetIDList1.Size = new System.Drawing.Size(1540, 135);
            this.JobDatasetIDList1.TabIndex = 0;
            // 
            // AboutTabPage
            // 
            this.AboutTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.AboutTabPage.Controls.Add(this.pnlAbout);
            this.AboutTabPage.Location = new System.Drawing.Point(4, 25);
            this.AboutTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AboutTabPage.Name = "AboutTabPage";
            this.AboutTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AboutTabPage.Size = new System.Drawing.Size(1548, 143);
            this.AboutTabPage.TabIndex = 7;
            this.AboutTabPage.Text = "About";
            // 
            // pnlAbout
            // 
            this.pnlAbout.BackColor = System.Drawing.SystemColors.Control;
            this.pnlAbout.Controls.Add(this.txtServer);
            this.pnlAbout.Controls.Add(this.txtVersion);
            this.pnlAbout.Controls.Add(this.lblAboutLink);
            this.pnlAbout.Controls.Add(this.txtAbout2);
            this.pnlAbout.Controls.Add(this.txtAbout1);
            this.pnlAbout.Controls.Add(this.txtAbout3);
            this.pnlAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAbout.Location = new System.Drawing.Point(4, 4);
            this.pnlAbout.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Size = new System.Drawing.Size(1540, 135);
            this.pnlAbout.TabIndex = 1;
            // 
            // lblAboutLink
            // 
            this.lblAboutLink.AutoSize = true;
            this.lblAboutLink.Location = new System.Drawing.Point(268, 95);
            this.lblAboutLink.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAboutLink.Name = "lblAboutLink";
            this.lblAboutLink.Size = new System.Drawing.Size(85, 17);
            this.lblAboutLink.TabIndex = 3;
            this.lblAboutLink.TabStop = true;
            this.lblAboutLink.Text = "lblAboutLink";
            // 
            // txtAbout2
            // 
            this.txtAbout2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAbout2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout2.Location = new System.Drawing.Point(16, 62);
            this.txtAbout2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAbout2.Name = "txtAbout2";
            this.txtAbout2.ReadOnly = true;
            this.txtAbout2.Size = new System.Drawing.Size(1603, 15);
            this.txtAbout2.TabIndex = 2;
            this.txtAbout2.Text = "Written by ...";
            // 
            // txtAbout1
            // 
            this.txtAbout1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAbout1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout1.Location = new System.Drawing.Point(16, 12);
            this.txtAbout1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAbout1.Multiline = true;
            this.txtAbout1.Name = "txtAbout1";
            this.txtAbout1.ReadOnly = true;
            this.txtAbout1.Size = new System.Drawing.Size(1603, 44);
            this.txtAbout1.TabIndex = 0;
            this.txtAbout1.Text = "Mage File Processor can ...";
            // 
            // txtAbout3
            // 
            this.txtAbout3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout3.Location = new System.Drawing.Point(16, 95);
            this.txtAbout3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAbout3.Multiline = true;
            this.txtAbout3.Name = "txtAbout3";
            this.txtAbout3.ReadOnly = true;
            this.txtAbout3.Size = new System.Drawing.Size(300, 20);
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
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.panel2.Size = new System.Drawing.Size(1572, 308);
            this.panel2.TabIndex = 5;
            // 
            // FileListDisplayControl
            // 
            this.FileListDisplayControl.AllowDisableShiftClickMode = true;
            this.FileListDisplayControl.AutoSizeColumnWidths = false;
            this.FileListDisplayControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileListDisplayControl.HeaderVisible = true;
            this.FileListDisplayControl.ItemBlockSize = 100;
            this.FileListDisplayControl.Location = new System.Drawing.Point(7, 136);
            this.FileListDisplayControl.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.FileListDisplayControl.MultiSelect = true;
            this.FileListDisplayControl.Name = "FileListDisplayControl";
            this.FileListDisplayControl.Notice = "";
            this.FileListDisplayControl.PageTitle = "Title";
            this.FileListDisplayControl.Size = new System.Drawing.Size(1556, 164);
            this.FileListDisplayControl.TabIndex = 12;
            // 
            // FileSourceTabs
            // 
            this.FileSourceTabs.Controls.Add(this.GetEntityFilesTabPage);
            this.FileSourceTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.FileSourceTabs.Location = new System.Drawing.Point(7, 6);
            this.FileSourceTabs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FileSourceTabs.Name = "FileSourceTabs";
            this.FileSourceTabs.SelectedIndex = 0;
            this.FileSourceTabs.Size = new System.Drawing.Size(1556, 130);
            this.FileSourceTabs.TabIndex = 11;
            // 
            // GetEntityFilesTabPage
            // 
            this.GetEntityFilesTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.GetEntityFilesTabPage.Controls.Add(this.EntityFilePanel1);
            this.GetEntityFilesTabPage.Location = new System.Drawing.Point(4, 25);
            this.GetEntityFilesTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GetEntityFilesTabPage.Name = "GetEntityFilesTabPage";
            this.GetEntityFilesTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GetEntityFilesTabPage.Size = new System.Drawing.Size(1548, 101);
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
            this.EntityFilePanel1.Location = new System.Drawing.Point(4, 4);
            this.EntityFilePanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.EntityFilePanel1.Name = "EntityFilePanel1";
            this.EntityFilePanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.EntityFilePanel1.SearchInSubfolders = "No";
            this.EntityFilePanel1.Size = new System.Drawing.Size(1540, 93);
            this.EntityFilePanel1.SubfolderSearchName = "*";
            this.EntityFilePanel1.TabIndex = 10;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.statusPanel1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 949);
            this.panel5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1572, 58);
            this.panel5.TabIndex = 18;
            // 
            // statusPanel1
            // 
            this.statusPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.statusPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusPanel1.EnableCancel = true;
            this.statusPanel1.Location = new System.Drawing.Point(0, 0);
            this.statusPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.statusPanel1.Name = "statusPanel1";
            this.statusPanel1.OwnerControl = this;
            this.statusPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.statusPanel1.ShowCancel = true;
            this.statusPanel1.Size = new System.Drawing.Size(1572, 58);
            this.statusPanel1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel3);
            this.splitContainer2.Size = new System.Drawing.Size(1572, 949);
            this.splitContainer2.SplitterDistance = 647;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 19;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.filePackageMgmtPanel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.panel3.Size = new System.Drawing.Size(1572, 297);
            this.panel3.TabIndex = 2;
            // 
            // filePackageMgmtPanel1
            // 
            this.filePackageMgmtPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filePackageMgmtPanel1.FileListLabelPrefix = null;
            this.filePackageMgmtPanel1.FileSourceList = null;
            this.filePackageMgmtPanel1.ListTitle = "File Package Contents";
            this.filePackageMgmtPanel1.Location = new System.Drawing.Point(7, 6);
            this.filePackageMgmtPanel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.filePackageMgmtPanel1.Name = "filePackageMgmtPanel1";
            this.filePackageMgmtPanel1.OutputFilePath = "";
            this.filePackageMgmtPanel1.Padding = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.filePackageMgmtPanel1.Size = new System.Drawing.Size(1558, 285);
            this.filePackageMgmtPanel1.TabIndex = 2;
            this.filePackageMgmtPanel1.TotalSizeDisplay = "";
            // 
            // txtServer
            // 
            this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtServer.Location = new System.Drawing.Point(857, 95);
            this.txtServer.Margin = new System.Windows.Forms.Padding(4);
            this.txtServer.Multiline = true;
            this.txtServer.Name = "txtServer";
            this.txtServer.ReadOnly = true;
            this.txtServer.Size = new System.Drawing.Size(190, 20);
            this.txtServer.TabIndex = 12;
            this.txtServer.Text = "Server";
            // 
            // txtVersion
            // 
            this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtVersion.Location = new System.Drawing.Point(1064, 95);
            this.txtVersion.Margin = new System.Windows.Forms.Padding(4);
            this.txtVersion.Multiline = true;
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(302, 20);
            this.txtVersion.TabIndex = 11;
            this.txtVersion.Text = "Version";
            // 
            // FilePackagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1572, 1007);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.panel5);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FilePackagerForm";
            this.Text = "Mage File Packager";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.EntityListSourceTabs.ResumeLayout(false);
            this.DataPackageDetailsTabPage.ResumeLayout(false);
            this.DataPackageDatasetsTabPage.ResumeLayout(false);
            this.DataPackageJobsTabPage.ResumeLayout(false);
            this.DatasetTabPage.ResumeLayout(false);
            this.DatasetIDTabPage.ResumeLayout(false);
            this.QueryTabPage.ResumeLayout(false);
            this.JobsFlexQueryTabPage.ResumeLayout(false);
            this.JobListTabPage.ResumeLayout(false);
            this.JobsFromDatasetIDTabPage.ResumeLayout(false);
            this.AboutTabPage.ResumeLayout(false);
            this.pnlAbout.ResumeLayout(false);
            this.pnlAbout.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.FileSourceTabs.ResumeLayout(false);
            this.GetEntityFilesTabPage.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private MageDisplayLib.GridViewDisplayControl JobListDisplayControl;
        private System.Windows.Forms.TabControl EntityListSourceTabs;
        private System.Windows.Forms.TabPage QueryTabPage;
        private MageUIComponents.JobListPanel JobListPanel1;
        private System.Windows.Forms.TabPage JobsFlexQueryTabPage;
        private MageUIComponents.FlexQueryPanel JobFlexQueryPanel;
        private System.Windows.Forms.TabPage JobListTabPage;
        private MageUIComponents.JobIDListPanel JobIDListPanel1;
        private System.Windows.Forms.TabPage JobsFromDatasetIDTabPage;
        private MageUIComponents.JobIDListPanel JobDatasetIDList1;
        private System.Windows.Forms.TabPage DataPackageJobsTabPage;
        private MageUIComponents.JobDataPackagePanel JobDataPackagePanel1;
        private System.Windows.Forms.TabPage DataPackageDatasetsTabPage;
        private MageUIComponents.JobDataPackagePanel JobDataPackagePanel2;
        private System.Windows.Forms.TabPage DatasetTabPage;
        private MageUIComponents.DatasetQueryPanel DatasetQueryPanel1;
        private System.Windows.Forms.TabPage DatasetIDTabPage;
        private MageUIComponents.DatasetIDListPanel DatasetIDListPanel1;
        private System.Windows.Forms.TabPage AboutTabPage;
        private System.Windows.Forms.Panel pnlAbout;
        private System.Windows.Forms.LinkLabel lblAboutLink;
        private System.Windows.Forms.TextBox txtAbout2;
        private System.Windows.Forms.TextBox txtAbout1;
        private System.Windows.Forms.TextBox txtAbout3;
        private System.Windows.Forms.Panel panel2;
        private MageDisplayLib.GridViewDisplayControl FileListDisplayControl;
        private System.Windows.Forms.TabControl FileSourceTabs;
        private System.Windows.Forms.TabPage GetEntityFilesTabPage;
        private MageUIComponents.EntityFilePanel EntityFilePanel1;
        private System.Windows.Forms.TabPage DataPackageDetailsTabPage;
        private MageUIComponents.DataPackageDetailsListPanel dataPackageDetailsListPanel1;
        private System.Windows.Forms.Panel panel5;
        private MageDisplayLib.StatusPanel statusPanel1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel3;
        private FilePackageMgmtPanel filePackageMgmtPanel1;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtVersion;
    }
}

