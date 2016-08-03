namespace MageUIComponents
{
    partial class FileProcessingPanel
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.ClearFilterBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.FilterParametersCtl = new System.Windows.Forms.Button();
            this.FilterSelectionCtl = new System.Windows.Forms.Label();
            this.SelectFilterBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.EditColumnMappingBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectColMapBtn = new System.Windows.Forms.Button();
            this.ColumnMapSelectionCtl = new System.Windows.Forms.Label();
            this.ClearColMapBtn = new System.Windows.Forms.Button();
            this.ProcessAllFilesCtl = new System.Windows.Forms.Button();
            this.ProcessSelectedFilesCtl = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.ProcessAllFilesCtl);
            this.panel3.Controls.Add(this.ProcessSelectedFilesCtl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(7, 6);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1255, 93);
            this.panel3.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.ClearFilterBtn);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.FilterParametersCtl);
            this.panel2.Controls.Add(this.FilterSelectionCtl);
            this.panel2.Controls.Add(this.SelectFilterBtn);
            this.panel2.Location = new System.Drawing.Point(7, 4);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1029, 40);
            this.panel2.TabIndex = 19;
            // 
            // ClearFilterBtn
            // 
            this.ClearFilterBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearFilterBtn.Location = new System.Drawing.Point(649, 5);
            this.ClearFilterBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ClearFilterBtn.Name = "ClearFilterBtn";
            this.ClearFilterBtn.Size = new System.Drawing.Size(177, 28);
            this.ClearFilterBtn.TabIndex = 17;
            this.ClearFilterBtn.Text = "Clear Filter";
            this.ClearFilterBtn.UseVisualStyleBackColor = true;
            this.ClearFilterBtn.Click += new System.EventHandler(this.ClearFilterBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filter";
            // 
            // FilterParametersCtl
            // 
            this.FilterParametersCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterParametersCtl.Location = new System.Drawing.Point(533, 5);
            this.FilterParametersCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FilterParametersCtl.Name = "FilterParametersCtl";
            this.FilterParametersCtl.Size = new System.Drawing.Size(108, 28);
            this.FilterParametersCtl.TabIndex = 9;
            this.FilterParametersCtl.Text = "Parameters...";
            this.FilterParametersCtl.UseVisualStyleBackColor = true;
            this.FilterParametersCtl.Click += new System.EventHandler(this.FilterParametersCtl_Click);
            // 
            // FilterSelectionCtl
            // 
            this.FilterSelectionCtl.AutoSize = true;
            this.FilterSelectionCtl.Location = new System.Drawing.Point(133, 11);
            this.FilterSelectionCtl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FilterSelectionCtl.Name = "FilterSelectionCtl";
            this.FilterSelectionCtl.Size = new System.Drawing.Size(58, 17);
            this.FilterSelectionCtl.TabIndex = 16;
            this.FilterSelectionCtl.Text = "All Pass";
            this.FilterSelectionCtl.TextChanged += new System.EventHandler(this.FilterSelectionCtl_TextChanged);
            // 
            // SelectFilterBtn
            // 
            this.SelectFilterBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectFilterBtn.Location = new System.Drawing.Point(835, 5);
            this.SelectFilterBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SelectFilterBtn.Name = "SelectFilterBtn";
            this.SelectFilterBtn.Size = new System.Drawing.Size(188, 28);
            this.SelectFilterBtn.TabIndex = 15;
            this.SelectFilterBtn.Text = "Select Filter...";
            this.SelectFilterBtn.UseVisualStyleBackColor = true;
            this.SelectFilterBtn.Click += new System.EventHandler(this.SelectFilterBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.EditColumnMappingBtn);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.SelectColMapBtn);
            this.panel1.Controls.Add(this.ColumnMapSelectionCtl);
            this.panel1.Controls.Add(this.ClearColMapBtn);
            this.panel1.Location = new System.Drawing.Point(7, 43);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1029, 41);
            this.panel1.TabIndex = 18;
            // 
            // EditColumnMappingBtn
            // 
            this.EditColumnMappingBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EditColumnMappingBtn.Location = new System.Drawing.Point(533, 4);
            this.EditColumnMappingBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.EditColumnMappingBtn.Name = "EditColumnMappingBtn";
            this.EditColumnMappingBtn.Size = new System.Drawing.Size(107, 28);
            this.EditColumnMappingBtn.TabIndex = 15;
            this.EditColumnMappingBtn.Text = "Edit...";
            this.EditColumnMappingBtn.UseVisualStyleBackColor = true;
            this.EditColumnMappingBtn.Click += new System.EventHandler(this.EditColumnMappingBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Column Mapping";
            // 
            // SelectColMapBtn
            // 
            this.SelectColMapBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectColMapBtn.Location = new System.Drawing.Point(835, 5);
            this.SelectColMapBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SelectColMapBtn.Name = "SelectColMapBtn";
            this.SelectColMapBtn.Size = new System.Drawing.Size(188, 28);
            this.SelectColMapBtn.TabIndex = 12;
            this.SelectColMapBtn.Text = "Select Column Mapping...";
            this.SelectColMapBtn.UseVisualStyleBackColor = true;
            this.SelectColMapBtn.Click += new System.EventHandler(this.SelectColMapBtn_Click);
            // 
            // ColumnMapSelectionCtl
            // 
            this.ColumnMapSelectionCtl.AutoSize = true;
            this.ColumnMapSelectionCtl.Location = new System.Drawing.Point(133, 11);
            this.ColumnMapSelectionCtl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ColumnMapSelectionCtl.Name = "ColumnMapSelectionCtl";
            this.ColumnMapSelectionCtl.Size = new System.Drawing.Size(79, 17);
            this.ColumnMapSelectionCtl.TabIndex = 13;
            this.ColumnMapSelectionCtl.Text = "(automatic)";
            // 
            // ClearColMapBtn
            // 
            this.ClearColMapBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearColMapBtn.Location = new System.Drawing.Point(649, 5);
            this.ClearColMapBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ClearColMapBtn.Name = "ClearColMapBtn";
            this.ClearColMapBtn.Size = new System.Drawing.Size(177, 28);
            this.ClearColMapBtn.TabIndex = 14;
            this.ClearColMapBtn.Text = "Clear Column Mapping";
            this.ClearColMapBtn.UseVisualStyleBackColor = true;
            this.ClearColMapBtn.Click += new System.EventHandler(this.ClearColMapBtn_Click);
            // 
            // ProcessAllFilesCtl
            // 
            this.ProcessAllFilesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessAllFilesCtl.Location = new System.Drawing.Point(1057, 41);
            this.ProcessAllFilesCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ProcessAllFilesCtl.Name = "ProcessAllFilesCtl";
            this.ProcessAllFilesCtl.Size = new System.Drawing.Size(187, 28);
            this.ProcessAllFilesCtl.TabIndex = 8;
            this.ProcessAllFilesCtl.Text = "&Process All Files";
            this.ProcessAllFilesCtl.UseVisualStyleBackColor = true;
            this.ProcessAllFilesCtl.Click += new System.EventHandler(this.ProcessAllFilesCtl_Click);
            // 
            // ProcessSelectedFilesCtl
            // 
            this.ProcessSelectedFilesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ProcessSelectedFilesCtl.Location = new System.Drawing.Point(1057, 10);
            this.ProcessSelectedFilesCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ProcessSelectedFilesCtl.Name = "ProcessSelectedFilesCtl";
            this.ProcessSelectedFilesCtl.Size = new System.Drawing.Size(187, 28);
            this.ProcessSelectedFilesCtl.TabIndex = 0;
            this.ProcessSelectedFilesCtl.Text = "Process Selected &Files";
            this.ProcessSelectedFilesCtl.UseVisualStyleBackColor = true;
            this.ProcessSelectedFilesCtl.Click += new System.EventHandler(this.ProcessSelectedFilesCtl_Click);
            // 
            // FileProcessingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FileProcessingPanel";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(1269, 105);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button ProcessAllFilesCtl;
        private System.Windows.Forms.Button ProcessSelectedFilesCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button FilterParametersCtl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SelectColMapBtn;
        private System.Windows.Forms.Label ColumnMapSelectionCtl;
        private System.Windows.Forms.Button ClearColMapBtn;
        private System.Windows.Forms.Button SelectFilterBtn;
        private System.Windows.Forms.Label FilterSelectionCtl;
        private System.Windows.Forms.Button ClearFilterBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button EditColumnMappingBtn;
    }
}
