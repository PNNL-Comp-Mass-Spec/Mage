namespace MageUIComponents {
    partial class FolderDestinationPanel {
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
            this.panel3 = new System.Windows.Forms.Panel();
			this.SelectFolderCtl = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.OutputFolderCtl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.OutputFileCtl = new System.Windows.Forms.TextBox();
            this.DefineDestinationFileCtl = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.DefineDestinationFileCtl);
            this.panel3.Controls.Add(this.SelectFolderCtl);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.OutputFileCtl);
            this.panel3.Controls.Add(this.OutputFolderCtl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(5, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(681, 58);
            this.panel3.TabIndex = 3;
            // 
            // SelectFolderCtl
            // 
            this.SelectFolderCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectFolderCtl.Location = new System.Drawing.Point(646, 6);
            this.SelectFolderCtl.Name = "SelectFolderCtl";
            this.SelectFolderCtl.Size = new System.Drawing.Size(30, 23);
            this.SelectFolderCtl.TabIndex = 6;
            this.SelectFolderCtl.Text = "...";
            this.SelectFolderCtl.UseVisualStyleBackColor = true;
            this.SelectFolderCtl.Click += new System.EventHandler(this.SelectFolderCtl_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Destination Folder";
            // 
            // OutputFolderCtl
            // 
            this.OutputFolderCtl.AcceptsReturn = true;
            this.OutputFolderCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputFolderCtl.Location = new System.Drawing.Point(100, 8);
            this.OutputFolderCtl.Name = "OutputFolderCtl";
            this.OutputFolderCtl.Size = new System.Drawing.Size(540, 20);
            this.OutputFolderCtl.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination File";
            // 
            // OutputFileCtl
            // 
            this.OutputFileCtl.AcceptsReturn = true;
            this.OutputFileCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputFileCtl.Location = new System.Drawing.Point(100, 30);
            this.OutputFileCtl.Name = "OutputFileCtl";
            this.OutputFileCtl.Size = new System.Drawing.Size(540, 20);
            this.OutputFileCtl.TabIndex = 1;
            // 
            // DefineDestinationFileCtl
            // 
            this.DefineDestinationFileCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefineDestinationFileCtl.Location = new System.Drawing.Point(646, 30);
            this.DefineDestinationFileCtl.Name = "DefineDestinationFileCtl";
            this.DefineDestinationFileCtl.Size = new System.Drawing.Size(30, 23);
            this.DefineDestinationFileCtl.TabIndex = 7;
            this.DefineDestinationFileCtl.Text = "...";
            this.DefineDestinationFileCtl.UseVisualStyleBackColor = true;
            this.DefineDestinationFileCtl.Click += new System.EventHandler(this.DefineDestinationFileCtl_Click);
            // 
            // FolderDestinationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Name = "FolderDestinationPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(691, 68);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button SelectFolderCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OutputFolderCtl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox OutputFileCtl;
		private System.Windows.Forms.Button DefineDestinationFileCtl;
    }
}
