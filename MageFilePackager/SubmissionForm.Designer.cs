namespace MageFilePackager {
    partial class SubmissionForm {
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
            this.CancelCtl = new System.Windows.Forms.Button();
            this.OkCtl = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.notificationEmailCtl = new System.Windows.Forms.TextBox();
            this.packageNameCtl = new System.Windows.Forms.TextBox();
            this.packageDescriptonCtl = new System.Windows.Forms.TextBox();
            this.urlCtl = new System.Windows.Forms.TextBox();
            this.saveToFileCtl = new System.Windows.Forms.CheckBox();
            this.sendToServerCtl = new System.Windows.Forms.CheckBox();
            this.manifestFilePathCtl = new System.Windows.Forms.TextBox();
            this.setFilePathBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CancelCtl
            // 
            this.CancelCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelCtl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelCtl.Location = new System.Drawing.Point(366, 295);
            this.CancelCtl.Name = "CancelCtl";
            this.CancelCtl.Size = new System.Drawing.Size(75, 23);
            this.CancelCtl.TabIndex = 3;
            this.CancelCtl.Text = "Cancel";
            this.CancelCtl.UseVisualStyleBackColor = true;
            // 
            // OkCtl
            // 
            this.OkCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkCtl.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkCtl.Location = new System.Drawing.Point(284, 295);
            this.OkCtl.Name = "OkCtl";
            this.OkCtl.Size = new System.Drawing.Size(75, 23);
            this.OkCtl.TabIndex = 2;
            this.OkCtl.Text = "OK";
            this.OkCtl.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Notification Email";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Package Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Package Description";
            // 
            // notificationEmailCtl
            // 
            this.notificationEmailCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notificationEmailCtl.Location = new System.Drawing.Point(124, 9);
            this.notificationEmailCtl.Name = "notificationEmailCtl";
            this.notificationEmailCtl.Size = new System.Drawing.Size(317, 20);
            this.notificationEmailCtl.TabIndex = 7;
            this.notificationEmailCtl.Text = "grkiebel@pnl.gov";
            // 
            // packageNameCtl
            // 
            this.packageNameCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packageNameCtl.Location = new System.Drawing.Point(124, 39);
            this.packageNameCtl.Name = "packageNameCtl";
            this.packageNameCtl.Size = new System.Drawing.Size(317, 20);
            this.packageNameCtl.TabIndex = 8;
            this.packageNameCtl.Text = "Test Package";
            // 
            // packageDescriptonCtl
            // 
            this.packageDescriptonCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packageDescriptonCtl.Location = new System.Drawing.Point(124, 70);
            this.packageDescriptonCtl.Multiline = true;
            this.packageDescriptonCtl.Name = "packageDescriptonCtl";
            this.packageDescriptonCtl.Size = new System.Drawing.Size(317, 135);
            this.packageDescriptonCtl.TabIndex = 9;
            this.packageDescriptonCtl.Text = "Software testing and development";
            // 
            // urlCtl
            // 
            this.urlCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlCtl.Location = new System.Drawing.Point(124, 257);
            this.urlCtl.Name = "urlCtl";
            this.urlCtl.Size = new System.Drawing.Size(317, 20);
            this.urlCtl.TabIndex = 11;
            this.urlCtl.Text = "http://dms2.pnl.gov/data_package_publish/submit";
            // 
            // saveToFileCtl
            // 
            this.saveToFileCtl.AutoSize = true;
            this.saveToFileCtl.Checked = true;
            this.saveToFileCtl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveToFileCtl.Location = new System.Drawing.Point(15, 225);
            this.saveToFileCtl.Name = "saveToFileCtl";
            this.saveToFileCtl.Size = new System.Drawing.Size(79, 17);
            this.saveToFileCtl.TabIndex = 12;
            this.saveToFileCtl.Text = "Save to file";
            this.saveToFileCtl.UseVisualStyleBackColor = true;
            // 
            // sendToServerCtl
            // 
            this.sendToServerCtl.AutoSize = true;
            this.sendToServerCtl.Location = new System.Drawing.Point(15, 257);
            this.sendToServerCtl.Name = "sendToServerCtl";
            this.sendToServerCtl.Size = new System.Drawing.Size(95, 17);
            this.sendToServerCtl.TabIndex = 13;
            this.sendToServerCtl.Text = "Send to server";
            this.sendToServerCtl.UseVisualStyleBackColor = true;
            // 
            // manifestFilePathCtl
            // 
            this.manifestFilePathCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.manifestFilePathCtl.Location = new System.Drawing.Point(124, 225);
            this.manifestFilePathCtl.Name = "manifestFilePathCtl";
            this.manifestFilePathCtl.Size = new System.Drawing.Size(279, 20);
            this.manifestFilePathCtl.TabIndex = 14;
            this.manifestFilePathCtl.Text = "C:\\Data\\Datapackage\\oink.xml";
            // 
            // setFilePathBtn
            // 
            this.setFilePathBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.setFilePathBtn.Location = new System.Drawing.Point(409, 223);
            this.setFilePathBtn.Name = "setFilePathBtn";
            this.setFilePathBtn.Size = new System.Drawing.Size(32, 23);
            this.setFilePathBtn.TabIndex = 15;
            this.setFilePathBtn.Text = "...";
            this.setFilePathBtn.UseVisualStyleBackColor = true;
            this.setFilePathBtn.Click += new System.EventHandler(this.SetFilePathBtnClick);
            // 
            // SubmissionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 321);
            this.Controls.Add(this.setFilePathBtn);
            this.Controls.Add(this.manifestFilePathCtl);
            this.Controls.Add(this.sendToServerCtl);
            this.Controls.Add(this.saveToFileCtl);
            this.Controls.Add(this.urlCtl);
            this.Controls.Add(this.packageDescriptonCtl);
            this.Controls.Add(this.packageNameCtl);
            this.Controls.Add(this.notificationEmailCtl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CancelCtl);
            this.Controls.Add(this.OkCtl);
            this.Name = "SubmissionForm";
            this.Text = "Submit File Package";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CancelCtl;
        private System.Windows.Forms.Button OkCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox notificationEmailCtl;
        private System.Windows.Forms.TextBox packageNameCtl;
        private System.Windows.Forms.TextBox packageDescriptonCtl;
        private System.Windows.Forms.TextBox urlCtl;
        private System.Windows.Forms.CheckBox saveToFileCtl;
        private System.Windows.Forms.CheckBox sendToServerCtl;
        private System.Windows.Forms.TextBox manifestFilePathCtl;
        private System.Windows.Forms.Button setFilePathBtn;
    }
}