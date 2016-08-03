namespace MageExtractor
{
    partial class ExtractorForm
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractorForm));
            this.EntityListSourceTabs = new System.Windows.Forms.TabControl();
            this.QueryTabPage = new System.Windows.Forms.TabPage();
            this.JobSimpleQueryPanel = new MageUIComponents.JobListPanel();
            this.JobsFlexQueryTabPage = new System.Windows.Forms.TabPage();
            this.JobFlexQueryPanel = new MageUIComponents.FlexQueryPanel();
            this.JobListTabPage = new System.Windows.Forms.TabPage();
            this.JobIDListPanel1 = new MageUIComponents.JobIDListPanel();
            this.JobsFromDatasetIDTabPage = new System.Windows.Forms.TabPage();
            this.JobDatasetIDList1 = new MageUIComponents.JobIDListPanel();
            this.DataPackageTabPage = new System.Windows.Forms.TabPage();
            this.JobDataPackagePanel1 = new MageUIComponents.JobDataPackagePanel();
            this.AboutTabPage = new System.Windows.Forms.TabPage();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.txtAbout3 = new System.Windows.Forms.TextBox();
            this.lblAboutLink = new System.Windows.Forms.LinkLabel();
            this.txtAbout2 = new System.Windows.Forms.TextBox();
            this.txtAbout1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.JobListDisplayCtl = new MageDisplayLib.GridViewDisplayControl();
            this.statusPanel1 = new MageDisplayLib.StatusPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.extractionSettingsPanel1 = new MageExtractor.ExtractionSettingsPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.FilterOutputTabs = new System.Windows.Forms.TabControl();
            this.ProcessFilesToLocalTabPage = new System.Windows.Forms.TabPage();
            this.FolderDestinationPanel1 = new MageUIComponents.FolderDestinationPanel();
            this.ProcessFileToSQLiteDBTabPage = new System.Windows.Forms.TabPage();
            this.SQLiteDestinationPanel1 = new MageUIComponents.SQLiteDestinationPanel();
            this.EntityListSourceTabs.SuspendLayout();
            this.QueryTabPage.SuspendLayout();
            this.JobsFlexQueryTabPage.SuspendLayout();
            this.JobListTabPage.SuspendLayout();
            this.JobsFromDatasetIDTabPage.SuspendLayout();
            this.DataPackageTabPage.SuspendLayout();
            this.AboutTabPage.SuspendLayout();
            this.pnlAbout.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.FilterOutputTabs.SuspendLayout();
            this.ProcessFilesToLocalTabPage.SuspendLayout();
            this.ProcessFileToSQLiteDBTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // EntityListSourceTabs
            // 
            this.EntityListSourceTabs.AccessibleDescription = " ";
            this.EntityListSourceTabs.Controls.Add(this.QueryTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobsFlexQueryTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobListTabPage);
            this.EntityListSourceTabs.Controls.Add(this.JobsFromDatasetIDTabPage);
            this.EntityListSourceTabs.Controls.Add(this.DataPackageTabPage);
            this.EntityListSourceTabs.Controls.Add(this.AboutTabPage);
            this.EntityListSourceTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.EntityListSourceTabs.Location = new System.Drawing.Point(0, 0);
            this.EntityListSourceTabs.Margin = new System.Windows.Forms.Padding(4);
            this.EntityListSourceTabs.Name = "EntityListSourceTabs";
            this.EntityListSourceTabs.SelectedIndex = 0;
            this.EntityListSourceTabs.Size = new System.Drawing.Size(1225, 138);
            this.EntityListSourceTabs.TabIndex = 6;
            this.EntityListSourceTabs.Tag = "About";
            // 
            // QueryTabPage
            // 
            this.QueryTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.QueryTabPage.Controls.Add(this.JobSimpleQueryPanel);
            this.QueryTabPage.Location = new System.Drawing.Point(4, 25);
            this.QueryTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.QueryTabPage.Name = "QueryTabPage";
            this.QueryTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.QueryTabPage.Size = new System.Drawing.Size(1217, 109);
            this.QueryTabPage.TabIndex = 0;
            this.QueryTabPage.Tag = "Jobs";
            this.QueryTabPage.Text = "Jobs From Query";
            // 
            // JobSimpleQueryPanel
            // 
            this.JobSimpleQueryPanel.BackColor = System.Drawing.SystemColors.Control;
            this.JobSimpleQueryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobSimpleQueryPanel.Location = new System.Drawing.Point(4, 4);
            this.JobSimpleQueryPanel.Margin = new System.Windows.Forms.Padding(5);
            this.JobSimpleQueryPanel.Name = "JobSimpleQueryPanel";
            this.JobSimpleQueryPanel.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobSimpleQueryPanel.Size = new System.Drawing.Size(1209, 101);
            this.JobSimpleQueryPanel.TabIndex = 0;
            // 
            // JobsFlexQueryTabPage
            // 
            this.JobsFlexQueryTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.JobsFlexQueryTabPage.Controls.Add(this.JobFlexQueryPanel);
            this.JobsFlexQueryTabPage.Location = new System.Drawing.Point(4, 25);
            this.JobsFlexQueryTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.JobsFlexQueryTabPage.Name = "JobsFlexQueryTabPage";
            this.JobsFlexQueryTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.JobsFlexQueryTabPage.Size = new System.Drawing.Size(1217, 109);
            this.JobsFlexQueryTabPage.TabIndex = 6;
            this.JobsFlexQueryTabPage.Tag = "Job_Flex_Query";
            this.JobsFlexQueryTabPage.Text = "Jobs From Flex Query";
            // 
            // JobFlexQueryPanel
            // 
            this.JobFlexQueryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobFlexQueryPanel.Location = new System.Drawing.Point(4, 4);
            this.JobFlexQueryPanel.Margin = new System.Windows.Forms.Padding(5);
            this.JobFlexQueryPanel.Name = "JobFlexQueryPanel";
            this.JobFlexQueryPanel.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobFlexQueryPanel.QueryName = null;
            this.JobFlexQueryPanel.Size = new System.Drawing.Size(1209, 101);
            this.JobFlexQueryPanel.TabIndex = 0;
            // 
            // JobListTabPage
            // 
            this.JobListTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.JobListTabPage.Controls.Add(this.JobIDListPanel1);
            this.JobListTabPage.Location = new System.Drawing.Point(4, 25);
            this.JobListTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.JobListTabPage.Name = "JobListTabPage";
            this.JobListTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.JobListTabPage.Size = new System.Drawing.Size(1217, 109);
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
            this.JobIDListPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.JobIDListPanel1.Name = "JobIDListPanel1";
            this.JobIDListPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobIDListPanel1.Size = new System.Drawing.Size(1209, 101);
            this.JobIDListPanel1.TabIndex = 0;
            // 
            // JobsFromDatasetIDTabPage
            // 
            this.JobsFromDatasetIDTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.JobsFromDatasetIDTabPage.Controls.Add(this.JobDatasetIDList1);
            this.JobsFromDatasetIDTabPage.Location = new System.Drawing.Point(4, 25);
            this.JobsFromDatasetIDTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.JobsFromDatasetIDTabPage.Name = "JobsFromDatasetIDTabPage";
            this.JobsFromDatasetIDTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.JobsFromDatasetIDTabPage.Size = new System.Drawing.Size(1217, 109);
            this.JobsFromDatasetIDTabPage.TabIndex = 5;
            this.JobsFromDatasetIDTabPage.Text = "Jobs From Dataset List";
            // 
            // JobDatasetIDList1
            // 
            this.JobDatasetIDList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDatasetIDList1.Legend = "(Dataset IDs)";
            this.JobDatasetIDList1.ListName = "Dataset_ID";
            this.JobDatasetIDList1.Location = new System.Drawing.Point(4, 4);
            this.JobDatasetIDList1.Margin = new System.Windows.Forms.Padding(5);
            this.JobDatasetIDList1.Name = "JobDatasetIDList1";
            this.JobDatasetIDList1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobDatasetIDList1.Size = new System.Drawing.Size(1209, 101);
            this.JobDatasetIDList1.TabIndex = 0;
            // 
            // DataPackageTabPage
            // 
            this.DataPackageTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.DataPackageTabPage.Controls.Add(this.JobDataPackagePanel1);
            this.DataPackageTabPage.Location = new System.Drawing.Point(4, 25);
            this.DataPackageTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.DataPackageTabPage.Name = "DataPackageTabPage";
            this.DataPackageTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.DataPackageTabPage.Size = new System.Drawing.Size(1217, 109);
            this.DataPackageTabPage.TabIndex = 2;
            this.DataPackageTabPage.Text = "Jobs From Data Package";
            // 
            // JobDataPackagePanel1
            // 
            this.JobDataPackagePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobDataPackagePanel1.Location = new System.Drawing.Point(4, 4);
            this.JobDataPackagePanel1.Margin = new System.Windows.Forms.Padding(5);
            this.JobDataPackagePanel1.Name = "JobDataPackagePanel1";
            this.JobDataPackagePanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.JobDataPackagePanel1.ShowGetDatasets = false;
            this.JobDataPackagePanel1.ShowGetJobs = true;
            this.JobDataPackagePanel1.Size = new System.Drawing.Size(1209, 101);
            this.JobDataPackagePanel1.TabIndex = 0;
            // 
            // AboutTabPage
            // 
            this.AboutTabPage.Controls.Add(this.pnlAbout);
            this.AboutTabPage.Location = new System.Drawing.Point(4, 25);
            this.AboutTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.AboutTabPage.Name = "AboutTabPage";
            this.AboutTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.AboutTabPage.Size = new System.Drawing.Size(1217, 109);
            this.AboutTabPage.TabIndex = 7;
            this.AboutTabPage.Text = "About";
            this.AboutTabPage.UseVisualStyleBackColor = true;
            // 
            // pnlAbout
            // 
            this.pnlAbout.BackColor = System.Drawing.SystemColors.Control;
            this.pnlAbout.Controls.Add(this.txtServer);
            this.pnlAbout.Controls.Add(this.txtVersion);
            this.pnlAbout.Controls.Add(this.txtAbout3);
            this.pnlAbout.Controls.Add(this.lblAboutLink);
            this.pnlAbout.Controls.Add(this.txtAbout2);
            this.pnlAbout.Controls.Add(this.txtAbout1);
            this.pnlAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAbout.Location = new System.Drawing.Point(4, 4);
            this.pnlAbout.Margin = new System.Windows.Forms.Padding(4);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Size = new System.Drawing.Size(1209, 101);
            this.pnlAbout.TabIndex = 0;
            // 
            // txtServer
            // 
            this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtServer.Location = new System.Drawing.Point(696, 78);
            this.txtServer.Margin = new System.Windows.Forms.Padding(4);
            this.txtServer.Multiline = true;
            this.txtServer.Name = "txtServer";
            this.txtServer.ReadOnly = true;
            this.txtServer.Size = new System.Drawing.Size(190, 20);
            this.txtServer.TabIndex = 10;
            this.txtServer.Text = "Server";
            // 
            // txtVersion
            // 
            this.txtVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtVersion.Location = new System.Drawing.Point(903, 78);
            this.txtVersion.Margin = new System.Windows.Forms.Padding(4);
            this.txtVersion.Multiline = true;
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.ReadOnly = true;
            this.txtVersion.Size = new System.Drawing.Size(302, 20);
            this.txtVersion.TabIndex = 9;
            this.txtVersion.Text = "Version";
            // 
            // txtAbout3
            // 
            this.txtAbout3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout3.Location = new System.Drawing.Point(16, 78);
            this.txtAbout3.Margin = new System.Windows.Forms.Padding(4);
            this.txtAbout3.Multiline = true;
            this.txtAbout3.Name = "txtAbout3";
            this.txtAbout3.ReadOnly = true;
            this.txtAbout3.Size = new System.Drawing.Size(229, 20);
            this.txtAbout3.TabIndex = 4;
            this.txtAbout3.Text = "For usage instructions, please see";
            // 
            // lblAboutLink
            // 
            this.lblAboutLink.AutoSize = true;
            this.lblAboutLink.Location = new System.Drawing.Point(253, 77);
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
            this.txtAbout2.Location = new System.Drawing.Point(16, 54);
            this.txtAbout2.Margin = new System.Windows.Forms.Padding(4);
            this.txtAbout2.Multiline = true;
            this.txtAbout2.Name = "txtAbout2";
            this.txtAbout2.ReadOnly = true;
            this.txtAbout2.Size = new System.Drawing.Size(1174, 18);
            this.txtAbout2.TabIndex = 2;
            this.txtAbout2.Text = "Written by ...";
            // 
            // txtAbout1
            // 
            this.txtAbout1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAbout1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAbout1.Location = new System.Drawing.Point(16, 12);
            this.txtAbout1.Margin = new System.Windows.Forms.Padding(4);
            this.txtAbout1.Multiline = true;
            this.txtAbout1.Name = "txtAbout1";
            this.txtAbout1.ReadOnly = true;
            this.txtAbout1.Size = new System.Drawing.Size(1174, 44);
            this.txtAbout1.TabIndex = 0;
            this.txtAbout1.Text = "Mage Extractor can ...";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.JobListDisplayCtl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 138);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.panel1.Size = new System.Drawing.Size(1225, 239);
            this.panel1.TabIndex = 7;
            // 
            // JobListDisplayCtl
            // 
            this.JobListDisplayCtl.AllowDisableShiftClickMode = true;
            this.JobListDisplayCtl.AutoSizeColumnWidths = false;
            this.JobListDisplayCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JobListDisplayCtl.HeaderVisible = true;
            this.JobListDisplayCtl.ItemBlockSize = 100;
            this.JobListDisplayCtl.Location = new System.Drawing.Point(7, 6);
            this.JobListDisplayCtl.Margin = new System.Windows.Forms.Padding(5);
            this.JobListDisplayCtl.MultiSelect = true;
            this.JobListDisplayCtl.Name = "JobListDisplayCtl";
            this.JobListDisplayCtl.Notice = "";
            this.JobListDisplayCtl.PageTitle = "Jobs";
            this.JobListDisplayCtl.Size = new System.Drawing.Size(1211, 227);
            this.JobListDisplayCtl.TabIndex = 1;
            // 
            // statusPanel1
            // 
            this.statusPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusPanel1.EnableCancel = true;
            this.statusPanel1.Location = new System.Drawing.Point(0, 245);
            this.statusPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.statusPanel1.Name = "statusPanel1";
            this.statusPanel1.OwnerControl = this;
            this.statusPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.statusPanel1.ShowCancel = true;
            this.statusPanel1.Size = new System.Drawing.Size(1225, 52);
            this.statusPanel1.TabIndex = 8;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.extractionSettingsPanel1);
            this.panel5.Controls.Add(this.statusPanel1);
            this.panel5.Controls.Add(this.panel4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 377);
            this.panel5.Margin = new System.Windows.Forms.Padding(4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1225, 297);
            this.panel5.TabIndex = 19;
            // 
            // extractionSettingsPanel1
            // 
            this.extractionSettingsPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extractionSettingsPanel1.KeepAllResults = "No";
            this.extractionSettingsPanel1.Location = new System.Drawing.Point(0, 135);
            this.extractionSettingsPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.extractionSettingsPanel1.MSGFCutoff = "1E-8";
            this.extractionSettingsPanel1.Name = "extractionSettingsPanel1";
            this.extractionSettingsPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.extractionSettingsPanel1.ResultFilterSetID = "All Pass";
            this.extractionSettingsPanel1.ResultTypeName = "Sequest Synopsis";
            this.extractionSettingsPanel1.Size = new System.Drawing.Size(1225, 110);
            this.extractionSettingsPanel1.TabIndex = 18;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.FilterOutputTabs);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.panel4.Size = new System.Drawing.Size(1225, 135);
            this.panel4.TabIndex = 17;
            // 
            // FilterOutputTabs
            // 
            this.FilterOutputTabs.Controls.Add(this.ProcessFilesToLocalTabPage);
            this.FilterOutputTabs.Controls.Add(this.ProcessFileToSQLiteDBTabPage);
            this.FilterOutputTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilterOutputTabs.Location = new System.Drawing.Point(7, 6);
            this.FilterOutputTabs.Margin = new System.Windows.Forms.Padding(4);
            this.FilterOutputTabs.Name = "FilterOutputTabs";
            this.FilterOutputTabs.SelectedIndex = 0;
            this.FilterOutputTabs.Size = new System.Drawing.Size(1211, 123);
            this.FilterOutputTabs.TabIndex = 15;
            // 
            // ProcessFilesToLocalTabPage
            // 
            this.ProcessFilesToLocalTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.ProcessFilesToLocalTabPage.Controls.Add(this.FolderDestinationPanel1);
            this.ProcessFilesToLocalTabPage.Location = new System.Drawing.Point(4, 25);
            this.ProcessFilesToLocalTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ProcessFilesToLocalTabPage.Name = "ProcessFilesToLocalTabPage";
            this.ProcessFilesToLocalTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ProcessFilesToLocalTabPage.Size = new System.Drawing.Size(1203, 94);
            this.ProcessFilesToLocalTabPage.TabIndex = 0;
            this.ProcessFilesToLocalTabPage.Tag = "File_Output";
            this.ProcessFilesToLocalTabPage.Text = "Extract Results To File";
            // 
            // FolderDestinationPanel1
            // 
            this.FolderDestinationPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.FolderDestinationPanel1.Location = new System.Drawing.Point(4, 4);
            this.FolderDestinationPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.FolderDestinationPanel1.Name = "FolderDestinationPanel1";
            this.FolderDestinationPanel1.OutputFile = "";
            this.FolderDestinationPanel1.OutputFolder = "C:\\Data\\Junk";
            this.FolderDestinationPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.FolderDestinationPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FolderDestinationPanel1.Size = new System.Drawing.Size(1195, 86);
            this.FolderDestinationPanel1.TabIndex = 13;
            // 
            // ProcessFileToSQLiteDBTabPage
            // 
            this.ProcessFileToSQLiteDBTabPage.BackColor = System.Drawing.SystemColors.Control;
            this.ProcessFileToSQLiteDBTabPage.Controls.Add(this.SQLiteDestinationPanel1);
            this.ProcessFileToSQLiteDBTabPage.Location = new System.Drawing.Point(4, 25);
            this.ProcessFileToSQLiteDBTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.ProcessFileToSQLiteDBTabPage.Name = "ProcessFileToSQLiteDBTabPage";
            this.ProcessFileToSQLiteDBTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.ProcessFileToSQLiteDBTabPage.Size = new System.Drawing.Size(1203, 94);
            this.ProcessFileToSQLiteDBTabPage.TabIndex = 1;
            this.ProcessFileToSQLiteDBTabPage.Tag = "SQLite_Output";
            this.ProcessFileToSQLiteDBTabPage.Text = "Extract Results To SQLite Database";
            // 
            // SQLiteDestinationPanel1
            // 
            this.SQLiteDestinationPanel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.SQLiteDestinationPanel1.DatabaseName = "C:\\Data\\test.db";
            this.SQLiteDestinationPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SQLiteDestinationPanel1.Location = new System.Drawing.Point(4, 4);
            this.SQLiteDestinationPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.SQLiteDestinationPanel1.Name = "SQLiteDestinationPanel1";
            this.SQLiteDestinationPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.SQLiteDestinationPanel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SQLiteDestinationPanel1.Size = new System.Drawing.Size(1195, 86);
            this.SQLiteDestinationPanel1.TabIndex = 14;
            this.SQLiteDestinationPanel1.TableName = "Extracted_Results";
            // 
            // ExtractorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1225, 674);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.EntityListSourceTabs);
            this.Controls.Add(this.panel5);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ExtractorForm";
            this.Text = "Mage Extractor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClearTempFiles);
            this.EntityListSourceTabs.ResumeLayout(false);
            this.QueryTabPage.ResumeLayout(false);
            this.JobsFlexQueryTabPage.ResumeLayout(false);
            this.JobListTabPage.ResumeLayout(false);
            this.JobsFromDatasetIDTabPage.ResumeLayout(false);
            this.DataPackageTabPage.ResumeLayout(false);
            this.AboutTabPage.ResumeLayout(false);
            this.pnlAbout.ResumeLayout(false);
            this.pnlAbout.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.FilterOutputTabs.ResumeLayout(false);
            this.ProcessFilesToLocalTabPage.ResumeLayout(false);
            this.ProcessFileToSQLiteDBTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl EntityListSourceTabs;
        private System.Windows.Forms.TabPage QueryTabPage;
        private MageUIComponents.JobListPanel JobSimpleQueryPanel;
        private System.Windows.Forms.TabPage JobListTabPage;
        private MageUIComponents.JobIDListPanel JobIDListPanel1;
        private System.Windows.Forms.TabPage JobsFromDatasetIDTabPage;
        private MageUIComponents.JobIDListPanel JobDatasetIDList1;
        private System.Windows.Forms.TabPage DataPackageTabPage;
        private MageUIComponents.JobDataPackagePanel JobDataPackagePanel1;
        private System.Windows.Forms.Panel panel1;
        private MageDisplayLib.GridViewDisplayControl JobListDisplayCtl;
        private System.Windows.Forms.TabPage JobsFlexQueryTabPage;
        private MageUIComponents.FlexQueryPanel JobFlexQueryPanel;
        private System.Windows.Forms.TabPage AboutTabPage;
        private System.Windows.Forms.Panel pnlAbout;
        private System.Windows.Forms.TextBox txtAbout1;
        private System.Windows.Forms.TextBox txtAbout2;
        private System.Windows.Forms.LinkLabel lblAboutLink;
        private System.Windows.Forms.TextBox txtAbout3;
        private MageDisplayLib.StatusPanel statusPanel1;
        private System.Windows.Forms.Panel panel5;
        private ExtractionSettingsPanel extractionSettingsPanel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TabControl FilterOutputTabs;
        private System.Windows.Forms.TabPage ProcessFilesToLocalTabPage;
        private MageUIComponents.FolderDestinationPanel FolderDestinationPanel1;
        private System.Windows.Forms.TabPage ProcessFileToSQLiteDBTabPage;
        private MageUIComponents.SQLiteDestinationPanel SQLiteDestinationPanel1;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtVersion;
    }
}