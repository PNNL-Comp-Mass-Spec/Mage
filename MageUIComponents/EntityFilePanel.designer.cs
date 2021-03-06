﻿namespace MageUIComponents
{
    partial class EntityFilePanel
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SearchGroupBoxCtl = new System.Windows.Forms.GroupBox();
            this.SubdirectorySearchNameCtl = new System.Windows.Forms.TextBox();
            this.SearchInSubdirectoriesCtl = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.IncludeDirectoriesCtl = new System.Windows.Forms.CheckBox();
            this.IncludefilesCtl = new System.Windows.Forms.CheckBox();
            this.FileSelectorRadioGroupBoxCtl = new System.Windows.Forms.GroupBox();
            this.FileSearchRadioBtn = new System.Windows.Forms.RadioButton();
            this.RegExRadioBtn = new System.Windows.Forms.RadioButton();
            this.GetFilesForAllEntriesCtl = new System.Windows.Forms.Button();
            this.GetFilesForSelectedEntriesCtl = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.FileSelectorsCtl = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SearchGroupBoxCtl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.FileSelectorRadioGroupBoxCtl.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.SearchGroupBoxCtl);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.FileSelectorRadioGroupBoxCtl);
            this.panel1.Controls.Add(this.GetFilesForAllEntriesCtl);
            this.panel1.Controls.Add(this.GetFilesForSelectedEntriesCtl);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.FileSelectorsCtl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(7, 6);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1157, 80);
            this.panel1.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(19, 37);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(418, 39);
            this.textBox1.TabIndex = 14;
            this.textBox1.Text = "Multiple file filters can be used (separate with a semicolon)\r\nRelative paths are" +
    " supported, for example ..\\*CacheInfo.txt";
            // 
            // SearchGroupBoxCtl
            // 
            this.SearchGroupBoxCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchGroupBoxCtl.Controls.Add(this.SubdirectorySearchNameCtl);
            this.SearchGroupBoxCtl.Controls.Add(this.SearchInSubdirectoriesCtl);
            this.SearchGroupBoxCtl.Location = new System.Drawing.Point(456, 1);
            this.SearchGroupBoxCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SearchGroupBoxCtl.Name = "SearchGroupBoxCtl";
            this.SearchGroupBoxCtl.Padding = new System.Windows.Forms.Padding(4);
            this.SearchGroupBoxCtl.Size = new System.Drawing.Size(179, 66);
            this.SearchGroupBoxCtl.TabIndex = 13;
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
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.IncludeDirectoriesCtl);
            this.groupBox1.Controls.Add(this.IncludefilesCtl);
            this.groupBox1.Location = new System.Drawing.Point(639, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(149, 66);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // IncludeDirectoriesCtl
            // 
            this.IncludeDirectoriesCtl.AutoSize = true;
            this.IncludeDirectoriesCtl.Location = new System.Drawing.Point(13, 41);
            this.IncludeDirectoriesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.IncludeDirectoriesCtl.Name = "IncludeDirectoriesCtl";
            this.IncludeDirectoriesCtl.Size = new System.Drawing.Size(147, 21);
            this.IncludeDirectoriesCtl.TabIndex = 1;
            this.IncludeDirectoriesCtl.Text = "Include Directories";
            this.IncludeDirectoriesCtl.UseVisualStyleBackColor = true;
            // 
            // IncludefilesCtl
            // 
            this.IncludefilesCtl.AutoSize = true;
            this.IncludefilesCtl.Checked = true;
            this.IncludefilesCtl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IncludefilesCtl.Location = new System.Drawing.Point(13, 12);
            this.IncludefilesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.IncludefilesCtl.Name = "IncludefilesCtl";
            this.IncludefilesCtl.Size = new System.Drawing.Size(108, 21);
            this.IncludefilesCtl.TabIndex = 0;
            this.IncludefilesCtl.Text = "Include Files";
            this.IncludefilesCtl.UseVisualStyleBackColor = true;
            // 
            // FileSelectorRadioGroupBoxCtl
            // 
            this.FileSelectorRadioGroupBoxCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FileSelectorRadioGroupBoxCtl.Controls.Add(this.FileSearchRadioBtn);
            this.FileSelectorRadioGroupBoxCtl.Controls.Add(this.RegExRadioBtn);
            this.FileSelectorRadioGroupBoxCtl.Location = new System.Drawing.Point(792, 1);
            this.FileSelectorRadioGroupBoxCtl.Margin = new System.Windows.Forms.Padding(4);
            this.FileSelectorRadioGroupBoxCtl.Name = "FileSelectorRadioGroupBoxCtl";
            this.FileSelectorRadioGroupBoxCtl.Padding = new System.Windows.Forms.Padding(4);
            this.FileSelectorRadioGroupBoxCtl.Size = new System.Drawing.Size(121, 66);
            this.FileSelectorRadioGroupBoxCtl.TabIndex = 10;
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
            // GetFilesForAllEntriesCtl
            // 
            this.GetFilesForAllEntriesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetFilesForAllEntriesCtl.Location = new System.Drawing.Point(921, 41);
            this.GetFilesForAllEntriesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.GetFilesForAllEntriesCtl.Name = "GetFilesForAllEntriesCtl";
            this.GetFilesForAllEntriesCtl.Size = new System.Drawing.Size(229, 28);
            this.GetFilesForAllEntriesCtl.TabIndex = 9;
            this.GetFilesForAllEntriesCtl.Text = "Find Files For &All Entities";
            this.GetFilesForAllEntriesCtl.UseVisualStyleBackColor = true;
            this.GetFilesForAllEntriesCtl.Click += new System.EventHandler(this.GetFilesForAllEntriesCtl_Click);
            // 
            // GetFilesForSelectedEntriesCtl
            // 
            this.GetFilesForSelectedEntriesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetFilesForSelectedEntriesCtl.Location = new System.Drawing.Point(921, 5);
            this.GetFilesForSelectedEntriesCtl.Margin = new System.Windows.Forms.Padding(4);
            this.GetFilesForSelectedEntriesCtl.Name = "GetFilesForSelectedEntriesCtl";
            this.GetFilesForSelectedEntriesCtl.Size = new System.Drawing.Size(229, 28);
            this.GetFilesForSelectedEntriesCtl.TabIndex = 8;
            this.GetFilesForSelectedEntriesCtl.Text = "Find Files For &Selected Entities";
            this.GetFilesForSelectedEntriesCtl.UseVisualStyleBackColor = true;
            this.GetFilesForSelectedEntriesCtl.Click += new System.EventHandler(this.GetFilesForSelectedEntriesCtl_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "File Name Filter";
            // 
            // FileSelectorsCtl
            // 
            this.FileSelectorsCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileSelectorsCtl.Location = new System.Drawing.Point(117, 7);
            this.FileSelectorsCtl.Margin = new System.Windows.Forms.Padding(4);
            this.FileSelectorsCtl.Name = "FileSelectorsCtl";
            this.FileSelectorsCtl.Size = new System.Drawing.Size(329, 22);
            this.FileSelectorsCtl.TabIndex = 6;
            // 
            // EntityFilePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "EntityFilePanel";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(1171, 92);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.SearchGroupBoxCtl.ResumeLayout(false);
            this.SearchGroupBoxCtl.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.FileSelectorRadioGroupBoxCtl.ResumeLayout(false);
            this.FileSelectorRadioGroupBoxCtl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button GetFilesForAllEntriesCtl;
        private System.Windows.Forms.Button GetFilesForSelectedEntriesCtl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FileSelectorsCtl;
        private System.Windows.Forms.GroupBox FileSelectorRadioGroupBoxCtl;
        private System.Windows.Forms.RadioButton FileSearchRadioBtn;
        private System.Windows.Forms.RadioButton RegExRadioBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox IncludefilesCtl;
        private System.Windows.Forms.CheckBox IncludeDirectoriesCtl;
        private System.Windows.Forms.GroupBox SearchGroupBoxCtl;
        private System.Windows.Forms.CheckBox SearchInSubdirectoriesCtl;
        private System.Windows.Forms.TextBox SubdirectorySearchNameCtl;
        private System.Windows.Forms.TextBox textBox1;
    }
}
