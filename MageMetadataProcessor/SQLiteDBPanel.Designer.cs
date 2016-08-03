namespace MageMetadataProcessor
{
    partial class SQLiteDBPanel
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
            this.TableNameCtl = new System.Windows.Forms.TextBox();
            this.DBFilePathCtl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveAllBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BrowseForFileBtn = new System.Windows.Forms.Button();
            this.SaveSelectedBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ClearColMapBtn = new System.Windows.Forms.Button();
            this.ColumnMapSelectionCtl = new System.Windows.Forms.Label();
            this.SelectColMapBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableNameCtl
            // 
            this.TableNameCtl.Location = new System.Drawing.Point(43, 5);
            this.TableNameCtl.Name = "TableNameCtl";
            this.TableNameCtl.Size = new System.Drawing.Size(191, 20);
            this.TableNameCtl.TabIndex = 0;
            // 
            // DBFilePathCtl
            // 
            this.DBFilePathCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DBFilePathCtl.Location = new System.Drawing.Point(298, 5);
            this.DBFilePathCtl.Name = "DBFilePathCtl";
            this.DBFilePathCtl.Size = new System.Drawing.Size(326, 20);
            this.DBFilePathCtl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Table";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(240, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database";
            // 
            // SaveAllBtn
            // 
            this.SaveAllBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveAllBtn.Location = new System.Drawing.Point(680, 33);
            this.SaveAllBtn.Name = "SaveAllBtn";
            this.SaveAllBtn.Size = new System.Drawing.Size(100, 23);
            this.SaveAllBtn.TabIndex = 4;
            this.SaveAllBtn.Text = "Save All";
            this.SaveAllBtn.UseVisualStyleBackColor = true;
            this.SaveAllBtn.Click += new System.EventHandler(this.SaveAllBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ClearColMapBtn);
            this.panel1.Controls.Add(this.ColumnMapSelectionCtl);
            this.panel1.Controls.Add(this.SelectColMapBtn);
            this.panel1.Controls.Add(this.BrowseForFileBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.SaveSelectedBtn);
            this.panel1.Controls.Add(this.SaveAllBtn);
            this.panel1.Controls.Add(this.TableNameCtl);
            this.panel1.Controls.Add(this.DBFilePathCtl);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(785, 65);
            this.panel1.TabIndex = 5;
            // 
            // BrowseForFileBtn
            // 
            this.BrowseForFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseForFileBtn.Location = new System.Drawing.Point(630, 3);
            this.BrowseForFileBtn.Name = "BrowseForFileBtn";
            this.BrowseForFileBtn.Size = new System.Drawing.Size(33, 23);
            this.BrowseForFileBtn.TabIndex = 5;
            this.BrowseForFileBtn.Text = "...";
            this.BrowseForFileBtn.UseVisualStyleBackColor = true;
            this.BrowseForFileBtn.Click += new System.EventHandler(this.BrowseForFileBtn_Click);
            // 
            // SaveSelectedBtn
            // 
            this.SaveSelectedBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveSelectedBtn.Location = new System.Drawing.Point(680, 3);
            this.SaveSelectedBtn.Name = "SaveSelectedBtn";
            this.SaveSelectedBtn.Size = new System.Drawing.Size(100, 23);
            this.SaveSelectedBtn.TabIndex = 4;
            this.SaveSelectedBtn.Text = "Save Selected";
            this.SaveSelectedBtn.UseVisualStyleBackColor = true;
            this.SaveSelectedBtn.Click += new System.EventHandler(this.SaveSelectedBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Column Mapping";
            // 
            // ClearColMapBtn
            // 
            this.ClearColMapBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearColMapBtn.Location = new System.Drawing.Point(384, 33);
            this.ClearColMapBtn.Name = "ClearColMapBtn";
            this.ClearColMapBtn.Size = new System.Drawing.Size(133, 23);
            this.ClearColMapBtn.TabIndex = 17;
            this.ClearColMapBtn.Text = "Clear Column Mapping";
            this.ClearColMapBtn.UseVisualStyleBackColor = true;
            this.ClearColMapBtn.Click += new System.EventHandler(this.ClearColMapBtn_Click);
            // 
            // ColumnMapSelectionCtl
            // 
            this.ColumnMapSelectionCtl.AutoSize = true;
            this.ColumnMapSelectionCtl.Location = new System.Drawing.Point(98, 38);
            this.ColumnMapSelectionCtl.Name = "ColumnMapSelectionCtl";
            this.ColumnMapSelectionCtl.Size = new System.Drawing.Size(59, 13);
            this.ColumnMapSelectionCtl.TabIndex = 16;
            this.ColumnMapSelectionCtl.Text = "(automatic)";
            // 
            // SelectColMapBtn
            // 
            this.SelectColMapBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectColMapBtn.Location = new System.Drawing.Point(524, 33);
            this.SelectColMapBtn.Name = "SelectColMapBtn";
            this.SelectColMapBtn.Size = new System.Drawing.Size(139, 23);
            this.SelectColMapBtn.TabIndex = 15;
            this.SelectColMapBtn.Text = "Select Column Mapping...";
            this.SelectColMapBtn.UseVisualStyleBackColor = true;
            this.SelectColMapBtn.Click += new System.EventHandler(this.SelectColMapBtn_Click);
            // 
            // SQLiteDBPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Name = "SQLiteDBPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(795, 75);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TableNameCtl;
        private System.Windows.Forms.TextBox DBFilePathCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SaveAllBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BrowseForFileBtn;
        private System.Windows.Forms.Button SaveSelectedBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ClearColMapBtn;
        private System.Windows.Forms.Label ColumnMapSelectionCtl;
        private System.Windows.Forms.Button SelectColMapBtn;
    }
}
