namespace MageMetadataProcessor
{
    partial class MetadataProcessorForm
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.gridViewDisplayControl1 = new MageDisplayLib.GridViewDisplayControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.statusPanel1 = new MageDisplayLib.StatusPanel();
            this.sqLiteDBPanel1 = new MageMetadataProcessor.SQLiteDBPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.datasetFactorsPanel1 = new MageMetadataProcessor.DatasetFactorsPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rawQueryPanel1 = new MageMetadataProcessor.RawQueryPanel();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gridViewDisplayControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 162);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.panel2.Size = new System.Drawing.Size(1288, 295);
            this.panel2.TabIndex = 1;
            // 
            // gridViewDisplayControl1
            // 
            this.gridViewDisplayControl1.AllowDisableShiftClickMode = true;
            this.gridViewDisplayControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridViewDisplayControl1.HeaderVisible = true;
            this.gridViewDisplayControl1.ItemBlockSize = 25;
            this.gridViewDisplayControl1.Location = new System.Drawing.Point(7, 6);
            this.gridViewDisplayControl1.Margin = new System.Windows.Forms.Padding(5);
            this.gridViewDisplayControl1.MultiSelect = true;
            this.gridViewDisplayControl1.Name = "gridViewDisplayControl1";
            this.gridViewDisplayControl1.Notice = "";
            this.gridViewDisplayControl1.PageTitle = "Title";
            this.gridViewDisplayControl1.Size = new System.Drawing.Size(1274, 283);
            this.gridViewDisplayControl1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.statusPanel1);
            this.panel3.Controls.Add(this.sqLiteDBPanel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 457);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1288, 160);
            this.panel3.TabIndex = 2;
            // 
            // statusPanel1
            // 
            this.statusPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPanel1.EnableCancel = true;
            this.statusPanel1.Location = new System.Drawing.Point(0, 98);
            this.statusPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.statusPanel1.Name = "statusPanel1";
            this.statusPanel1.OwnerControl = this;
            this.statusPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.statusPanel1.ShowCancel = true;
            this.statusPanel1.Size = new System.Drawing.Size(1284, 50);
            this.statusPanel1.TabIndex = 0;
            // 
            // sqLiteDBPanel1
            // 
            this.sqLiteDBPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqLiteDBPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sqLiteDBPanel1.ColumnMapping = "(automatic)";
            this.sqLiteDBPanel1.DBFilePath = "";
            this.sqLiteDBPanel1.Location = new System.Drawing.Point(0, 6);
            this.sqLiteDBPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.sqLiteDBPanel1.Name = "sqLiteDBPanel1";
            this.sqLiteDBPanel1.OutputColumnList = null;
            this.sqLiteDBPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.sqLiteDBPanel1.Size = new System.Drawing.Size(1284, 92);
            this.sqLiteDBPanel1.TabIndex = 1;
            this.sqLiteDBPanel1.TableName = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1288, 162);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.datasetFactorsPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(1280, 133);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Dataset Factors";
            // 
            // datasetFactorsPanel1
            // 
            this.datasetFactorsPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.datasetFactorsPanel1.DataPackageNumber = "";
            this.datasetFactorsPanel1.DatasetName = "sarc_ms";
            this.datasetFactorsPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datasetFactorsPanel1.Location = new System.Drawing.Point(4, 4);
            this.datasetFactorsPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.datasetFactorsPanel1.Name = "datasetFactorsPanel1";
            this.datasetFactorsPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.datasetFactorsPanel1.Size = new System.Drawing.Size(1272, 125);
            this.datasetFactorsPanel1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.rawQueryPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(1280, 133);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Raw Query";
            // 
            // rawQueryPanel1
            // 
            this.rawQueryPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.rawQueryPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rawQueryPanel1.Location = new System.Drawing.Point(4, 4);
            this.rawQueryPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.rawQueryPanel1.Name = "rawQueryPanel1";
            this.rawQueryPanel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.rawQueryPanel1.RawSQL = "";
            this.rawQueryPanel1.Size = new System.Drawing.Size(1272, 125);
            this.rawQueryPanel1.TabIndex = 3;
            // 
            // MetadataProcessorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1288, 617);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel3);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MetadataProcessorForm";
            this.Text = "Mage Metadata Processor";
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private MageDisplayLib.StatusPanel statusPanel1;
        private SQLiteDBPanel sqLiteDBPanel1;
        private System.Windows.Forms.Panel panel3;
        private RawQueryPanel rawQueryPanel1;
        private DatasetFactorsPanel datasetFactorsPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private MageDisplayLib.GridViewDisplayControl gridViewDisplayControl1;
    }
}

