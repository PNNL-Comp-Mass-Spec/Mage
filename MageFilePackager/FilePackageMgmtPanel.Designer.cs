namespace MageFilePackager {
    partial class FilePackageMgmtPanel {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel1 = new System.Windows.Forms.Panel();
            this.EditPackageListBtn = new System.Windows.Forms.Button();
            this.AddFromTreeBtn = new System.Windows.Forms.Button();
            this.ClearBtn = new System.Windows.Forms.Button();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.ContentInfoCtl = new System.Windows.Forms.Label();
            this.AddAllFilesBtn = new System.Windows.Forms.Button();
            this.AddSelectedFilesBtn = new System.Windows.Forms.Button();
            this.packageListDisplayControl1 = new MageDisplayLib.GridViewDisplayControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.EditPackageListBtn);
            this.panel1.Controls.Add(this.AddFromTreeBtn);
            this.panel1.Controls.Add(this.ClearBtn);
            this.panel1.Controls.Add(this.SubmitBtn);
            this.panel1.Controls.Add(this.OpenBtn);
            this.panel1.Controls.Add(this.SaveBtn);
            this.panel1.Controls.Add(this.ContentInfoCtl);
            this.panel1.Controls.Add(this.AddAllFilesBtn);
            this.panel1.Controls.Add(this.AddSelectedFilesBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1028, 32);
            this.panel1.TabIndex = 0;
            // 
            // EditPackageListBtn
            // 
            this.EditPackageListBtn.Location = new System.Drawing.Point(324, 2);
            this.EditPackageListBtn.Name = "EditPackageListBtn";
            this.EditPackageListBtn.Size = new System.Drawing.Size(75, 23);
            this.EditPackageListBtn.TabIndex = 10;
            this.EditPackageListBtn.Text = "Edit...";
            this.EditPackageListBtn.UseVisualStyleBackColor = true;
            this.EditPackageListBtn.Click += new System.EventHandler(this.EditPackageListBtnClick);
            // 
            // AddFromTreeBtn
            // 
            this.AddFromTreeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddFromTreeBtn.Location = new System.Drawing.Point(742, 5);
            this.AddFromTreeBtn.Name = "AddFromTreeBtn";
            this.AddFromTreeBtn.Size = new System.Drawing.Size(75, 23);
            this.AddFromTreeBtn.TabIndex = 9;
            this.AddFromTreeBtn.Text = "Add...";
            this.AddFromTreeBtn.UseVisualStyleBackColor = true;
            this.AddFromTreeBtn.Click += new System.EventHandler(this.AddFromTreeBtnClick);
            // 
            // ClearBtn
            // 
            this.ClearBtn.Location = new System.Drawing.Point(3, 4);
            this.ClearBtn.Name = "ClearBtn";
            this.ClearBtn.Size = new System.Drawing.Size(75, 23);
            this.ClearBtn.TabIndex = 8;
            this.ClearBtn.Text = "Clear";
            this.ClearBtn.UseVisualStyleBackColor = true;
            this.ClearBtn.Click += new System.EventHandler(this.ClearBtnClick);
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Location = new System.Drawing.Point(430, 2);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 23);
            this.SubmitBtn.TabIndex = 7;
            this.SubmitBtn.Text = "Submit";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtnClick);
            // 
            // OpenBtn
            // 
            this.OpenBtn.Location = new System.Drawing.Point(84, 5);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.Size = new System.Drawing.Size(75, 23);
            this.OpenBtn.TabIndex = 6;
            this.OpenBtn.Text = "Open";
            this.OpenBtn.UseVisualStyleBackColor = true;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtnClick);
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(165, 5);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveBtn.TabIndex = 5;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtnClick);
            // 
            // ContentInfoCtl
            // 
            this.ContentInfoCtl.AutoSize = true;
            this.ContentInfoCtl.Location = new System.Drawing.Point(259, 10);
            this.ContentInfoCtl.Name = "ContentInfoCtl";
            this.ContentInfoCtl.Size = new System.Drawing.Size(23, 13);
            this.ContentInfoCtl.TabIndex = 2;
            this.ContentInfoCtl.Text = "MB";
            // 
            // AddAllFilesBtn
            // 
            this.AddAllFilesBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddAllFilesBtn.Location = new System.Drawing.Point(823, 5);
            this.AddAllFilesBtn.Name = "AddAllFilesBtn";
            this.AddAllFilesBtn.Size = new System.Drawing.Size(91, 23);
            this.AddAllFilesBtn.TabIndex = 1;
            this.AddAllFilesBtn.Text = "Add All Files";
            this.AddAllFilesBtn.UseVisualStyleBackColor = true;
            this.AddAllFilesBtn.Click += new System.EventHandler(this.AddAllFilesBtnClick);
            // 
            // AddSelectedFilesBtn
            // 
            this.AddSelectedFilesBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddSelectedFilesBtn.Location = new System.Drawing.Point(918, 5);
            this.AddSelectedFilesBtn.Name = "AddSelectedFilesBtn";
            this.AddSelectedFilesBtn.Size = new System.Drawing.Size(105, 23);
            this.AddSelectedFilesBtn.TabIndex = 0;
            this.AddSelectedFilesBtn.Text = "Add Selected Files";
            this.AddSelectedFilesBtn.UseVisualStyleBackColor = true;
            this.AddSelectedFilesBtn.Click += new System.EventHandler(this.AddSelectedFilesBtnClick);
            // 
            // packageListDisplayControl1
            // 
            this.packageListDisplayControl1.AllowDisableShiftClickMode = true;
            this.packageListDisplayControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.packageListDisplayControl1.HeaderVisible = true;
            this.packageListDisplayControl1.ItemBlockSize = 100;
            this.packageListDisplayControl1.Location = new System.Drawing.Point(5, 32);
            this.packageListDisplayControl1.MultiSelect = true;
            this.packageListDisplayControl1.Name = "packageListDisplayControl1";
            this.packageListDisplayControl1.Notice = "";
            this.packageListDisplayControl1.PageTitle = "Title";
            this.packageListDisplayControl1.Size = new System.Drawing.Size(1028, 200);
            this.packageListDisplayControl1.TabIndex = 1;
            // 
            // FilePackageMgmtPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.packageListDisplayControl1);
            this.Controls.Add(this.panel1);
            this.Name = "FilePackageMgmtPanel";
            this.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Size = new System.Drawing.Size(1038, 232);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddSelectedFilesBtn;
        private System.Windows.Forms.Button AddAllFilesBtn;
        private System.Windows.Forms.Label ContentInfoCtl;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button OpenBtn;
        private System.Windows.Forms.Button SubmitBtn;
        private MageDisplayLib.GridViewDisplayControl packageListDisplayControl1;
        private System.Windows.Forms.Button ClearBtn;
        private System.Windows.Forms.Button AddFromTreeBtn;
        private System.Windows.Forms.Button EditPackageListBtn;
    }
}
