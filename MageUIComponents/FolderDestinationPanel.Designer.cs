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
			this.DefineDestinationFileCtl = new System.Windows.Forms.Button();
			this.SelectFolderCtl = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.OutputFileCtl = new System.Windows.Forms.TextBox();
			this.OutputFolderCtl = new System.Windows.Forms.TextBox();
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
			this.panel3.Location = new System.Drawing.Point(7, 6);
			this.panel3.Margin = new System.Windows.Forms.Padding(4);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(907, 72);
			this.panel3.TabIndex = 3;
			// 
			// DefineDestinationFileCtl
			// 
			this.DefineDestinationFileCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.DefineDestinationFileCtl.Location = new System.Drawing.Point(861, 37);
			this.DefineDestinationFileCtl.Margin = new System.Windows.Forms.Padding(4);
			this.DefineDestinationFileCtl.Name = "DefineDestinationFileCtl";
			this.DefineDestinationFileCtl.Size = new System.Drawing.Size(40, 28);
			this.DefineDestinationFileCtl.TabIndex = 5;
			this.DefineDestinationFileCtl.Text = "...";
			this.DefineDestinationFileCtl.UseVisualStyleBackColor = true;
			this.DefineDestinationFileCtl.Click += new System.EventHandler(this.DefineDestinationFileCtl_Click);
			// 
			// SelectFolderCtl
			// 
			this.SelectFolderCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SelectFolderCtl.Location = new System.Drawing.Point(861, 7);
			this.SelectFolderCtl.Margin = new System.Windows.Forms.Padding(4);
			this.SelectFolderCtl.Name = "SelectFolderCtl";
			this.SelectFolderCtl.Size = new System.Drawing.Size(40, 28);
			this.SelectFolderCtl.TabIndex = 2;
			this.SelectFolderCtl.Text = "...";
			this.SelectFolderCtl.UseVisualStyleBackColor = true;
			this.SelectFolderCtl.Click += new System.EventHandler(this.SelectFolderCtl_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 39);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 17);
			this.label2.TabIndex = 3;
			this.label2.Text = "Destination File";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 12);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(123, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Destination Folder";
			// 
			// OutputFileCtl
			// 
			this.OutputFileCtl.AcceptsReturn = true;
			this.OutputFileCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.OutputFileCtl.Location = new System.Drawing.Point(133, 37);
			this.OutputFileCtl.Margin = new System.Windows.Forms.Padding(4);
			this.OutputFileCtl.Name = "OutputFileCtl";
			this.OutputFileCtl.Size = new System.Drawing.Size(719, 22);
			this.OutputFileCtl.TabIndex = 4;
			// 
			// OutputFolderCtl
			// 
			this.OutputFolderCtl.AcceptsReturn = true;
			this.OutputFolderCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.OutputFolderCtl.Location = new System.Drawing.Point(133, 10);
			this.OutputFolderCtl.Margin = new System.Windows.Forms.Padding(4);
			this.OutputFolderCtl.Name = "OutputFolderCtl";
			this.OutputFolderCtl.Size = new System.Drawing.Size(719, 22);
			this.OutputFolderCtl.TabIndex = 1;
			// 
			// FolderDestinationPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel3);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FolderDestinationPanel";
			this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
			this.Size = new System.Drawing.Size(921, 84);
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
