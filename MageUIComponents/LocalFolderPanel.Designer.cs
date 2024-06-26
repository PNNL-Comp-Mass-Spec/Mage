﻿namespace MageUIComponents
{
    partial class LocalFolderPanel
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.FileSelectorRadioGroupBoxCtl = new System.Windows.Forms.GroupBox();
            this.FileSearchRadioBtn = new System.Windows.Forms.RadioButton();
            this.RegExRadioBtn = new System.Windows.Forms.RadioButton();
            this.SearchGroupBoxCtl = new System.Windows.Forms.GroupBox();
            this.SubdirectorySearchNameCtl = new System.Windows.Forms.TextBox();
            this.SearchInSubdirectoriesCtl = new System.Windows.Forms.CheckBox();
            this.SelectDirectoryCtl = new System.Windows.Forms.Button();
            this.GetFilesCtl = new System.Windows.Forms.Button();
            this.LocalFileNameFilterCtl = new System.Windows.Forms.TextBox();
            this.LocalDirectoryCtl = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.FileSelectorRadioGroupBoxCtl.SuspendLayout();
            this.SearchGroupBoxCtl.SuspendLayout();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.FileSelectorRadioGroupBoxCtl);
            this.panel1.Controls.Add(this.SearchGroupBoxCtl);
            this.panel1.Controls.Add(this.SelectDirectoryCtl);
            this.panel1.Controls.Add(this.GetFilesCtl);
            this.panel1.Controls.Add(this.LocalFileNameFilterCtl);
            this.panel1.Controls.Add(this.LocalDirectoryCtl);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(7, 6);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 77);
            this.panel1.TabIndex = 6;
            //
            // FileSelectorRadioGroupBoxCtl
            //
            this.FileSelectorRadioGroupBoxCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FileSelectorRadioGroupBoxCtl.Controls.Add(this.FileSearchRadioBtn);
            this.FileSelectorRadioGroupBoxCtl.Controls.Add(this.RegExRadioBtn);
            this.FileSelectorRadioGroupBoxCtl.Location = new System.Drawing.Point(776, 1);
            this.FileSelectorRadioGroupBoxCtl.Margin = new System.Windows.Forms.Padding(4);
            this.FileSelectorRadioGroupBoxCtl.Name = "FileSelectorRadioGroupBoxCtl";
            this.FileSelectorRadioGroupBoxCtl.Padding = new System.Windows.Forms.Padding(4);
            this.FileSelectorRadioGroupBoxCtl.Size = new System.Drawing.Size(121, 66);
            this.FileSelectorRadioGroupBoxCtl.TabIndex = 15;
            this.FileSelectorRadioGroupBoxCtl.TabStop = false;
            //
            // FileSearchRadioBtn
            //
            this.FileSearchRadioBtn.AutoSize = true;
            this.FileSearchRadioBtn.Checked = true;
            this.FileSearchRadioBtn.Location = new System.Drawing.Point(13, 41);
            this.FileSearchRadioBtn.Margin = new System.Windows.Forms.Padding(4);
            this.FileSearchRadioBtn.Name = "FileSearchRadioBtn";
            this.FileSearchRadioBtn.Size = new System.Drawing.Size(100, 21);
            this.FileSearchRadioBtn.TabIndex = 1;
            this.FileSearchRadioBtn.TabStop = true;
            this.FileSearchRadioBtn.Text = "File Search";
            this.FileSearchRadioBtn.UseVisualStyleBackColor = true;
            this.FileSearchRadioBtn.CheckedChanged += new System.EventHandler(this.FileSearchRadioBtn_CheckedChanged);
            //
            // RegExRadioBtn
            //
            this.RegExRadioBtn.AutoSize = true;
            this.RegExRadioBtn.Location = new System.Drawing.Point(13, 12);
            this.RegExRadioBtn.Margin = new System.Windows.Forms.Padding(4);
            this.RegExRadioBtn.Name = "RegExRadioBtn";
            this.RegExRadioBtn.Size = new System.Drawing.Size(70, 21);
            this.RegExRadioBtn.TabIndex = 0;
            this.RegExRadioBtn.Text = "RegEx";
            this.RegExRadioBtn.UseVisualStyleBackColor = true;
            this.RegExRadioBtn.CheckedChanged += new System.EventHandler(this.RegExRadioBtn_CheckedChanged);
            //
            // SearchGroupBoxCtl
            //
            this.SearchGroupBoxCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchGroupBoxCtl.Controls.Add(this.SubdirectorySearchNameCtl);
            this.SearchGroupBoxCtl.Controls.Add(this.SearchInSubdirectoriesCtl);
            this.SearchGroupBoxCtl.Location = new System.Drawing.Point(589, 1);
            this.SearchGroupBoxCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SearchGroupBoxCtl.Name = "SearchGroupBoxCtl";
            this.SearchGroupBoxCtl.Padding = new System.Windows.Forms.Padding(4);
            this.SearchGroupBoxCtl.Size = new System.Drawing.Size(179, 66);
            this.SearchGroupBoxCtl.TabIndex = 14;
            this.SearchGroupBoxCtl.TabStop = false;
            //
            // SubdirectorySearchNameCtl
            //
            this.SubdirectorySearchNameCtl.Location = new System.Drawing.Point(13, 36);
            this.SubdirectorySearchNameCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SubdirectorySearchNameCtl.Name = "SubdirectorySearchNameCtl";
            this.SubdirectorySearchNameCtl.Size = new System.Drawing.Size(151, 22);
            this.SubdirectorySearchNameCtl.TabIndex = 1;
            this.SubdirectorySearchNameCtl.Text = "*";
            //
            // SearchInSubdirectoriesCtl
            //
            this.SearchInSubdirectoriesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchInSubdirectoriesCtl.AutoSize = true;
            this.SearchInSubdirectoriesCtl.Location = new System.Drawing.Point(13, 12);
            this.SearchInSubdirectoriesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SearchInSubdirectoriesCtl.Name = "SearchInSubdirectoriesCtl";
            this.SearchInSubdirectoriesCtl.Size = new System.Drawing.Size(168, 21);
            this.SearchInSubdirectoriesCtl.TabIndex = 0;
            this.SearchInSubdirectoriesCtl.Text = "Search subdirectories";
            this.SearchInSubdirectoriesCtl.UseVisualStyleBackColor = true;
            //
            // SelectDirectoryCtl
            //
            this.SelectDirectoryCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectDirectoryCtl.Location = new System.Drawing.Point(541, 7);
            this.SelectDirectoryCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SelectDirectoryCtl.Name = "SelectDirectoryCtl";
            this.SelectDirectoryCtl.Size = new System.Drawing.Size(40, 28);
            this.SelectDirectoryCtl.TabIndex = 5;
            this.SelectDirectoryCtl.Text = "...";
            this.SelectDirectoryCtl.UseVisualStyleBackColor = true;
            this.SelectDirectoryCtl.Click += new System.EventHandler(this.SelectDirectoryCtl_Click);
            //
            // GetFilesCtl
            //
            this.GetFilesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetFilesCtl.Location = new System.Drawing.Point(904, 21);
            this.GetFilesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.GetFilesCtl.Name = "GetFilesCtl";
            this.GetFilesCtl.Size = new System.Drawing.Size(113, 28);
            this.GetFilesCtl.TabIndex = 1;
            this.GetFilesCtl.Text = "Find Files";
            this.GetFilesCtl.UseVisualStyleBackColor = true;
            this.GetFilesCtl.Click += new System.EventHandler(this.GetFilesCtl_Click);
            //
            // LocalFileNameFilterCtl
            //
            this.LocalFileNameFilterCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalFileNameFilterCtl.Location = new System.Drawing.Point(120, 42);
            this.LocalFileNameFilterCtl.Margin = new System.Windows.Forms.Padding(4);
            this.LocalFileNameFilterCtl.Name = "LocalFileNameFilterCtl";
            this.LocalFileNameFilterCtl.Size = new System.Drawing.Size(461, 22);
            this.LocalFileNameFilterCtl.TabIndex = 4;
            //
            // LocalDirectoryCtl
            //
            this.LocalDirectoryCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalDirectoryCtl.Location = new System.Drawing.Point(120, 10);
            this.LocalDirectoryCtl.Margin = new System.Windows.Forms.Padding(4);
            this.LocalDirectoryCtl.Name = "LocalDirectoryCtl";
            this.LocalDirectoryCtl.Size = new System.Drawing.Size(413, 22);
            this.LocalDirectoryCtl.TabIndex = 4;
            //
            // textBox3
            //
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(100, -32);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(952, 22);
            this.textBox3.TabIndex = 3;
            //
            // label8
            //
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 45);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 17);
            this.label8.TabIndex = 2;
            this.label8.Text = "File Name Filter";
            //
            // label7
            //
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 13);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Local Directory";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, -90);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Input File";
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, -58);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Output File";
            //
            // textBox2
            //
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(100, -62);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(952, 22);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "C:\\Data\\test.txt";
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, -28);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "Filter Spec";
            //
            // textBox1
            //
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(100, -94);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(952, 22);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "C:\\Data\\syn\\Job_558940_sarc_OHSUdeptest_Glo_14_6Jan10_Doc_09-09-19_syn.txt";
            //
            // LocalFolderPanel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LocalFolderPanel";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(1038, 89);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.FileSelectorRadioGroupBoxCtl.ResumeLayout(false);
            this.FileSelectorRadioGroupBoxCtl.PerformLayout();
            this.SearchGroupBoxCtl.ResumeLayout(false);
            this.SearchGroupBoxCtl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox LocalFileNameFilterCtl;
        private System.Windows.Forms.TextBox LocalDirectoryCtl;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button GetFilesCtl;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SelectDirectoryCtl;
        private System.Windows.Forms.GroupBox SearchGroupBoxCtl;
        private System.Windows.Forms.TextBox SubdirectorySearchNameCtl;
        private System.Windows.Forms.CheckBox SearchInSubdirectoriesCtl;
        private System.Windows.Forms.GroupBox FileSelectorRadioGroupBoxCtl;
        private System.Windows.Forms.RadioButton FileSearchRadioBtn;
        private System.Windows.Forms.RadioButton RegExRadioBtn;
    }
}
