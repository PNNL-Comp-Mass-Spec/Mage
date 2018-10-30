namespace MageUIComponents
{
    partial class FolderDestinationPanel
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
            this.DefineDestinationFileCtl = new System.Windows.Forms.Button();
            this.SelectDirectoryCtl = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.OutputFileCtl = new System.Windows.Forms.TextBox();
            this.OutputDirectoryCtl = new System.Windows.Forms.TextBox();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.DefineDestinationFileCtl);
            this.panel3.Controls.Add(this.SelectDirectoryCtl);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.OutputFileCtl);
            this.panel3.Controls.Add(this.OutputDirectoryCtl);
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
            // SelectDirectoryCtl
            // 
            this.SelectDirectoryCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectDirectoryCtl.Location = new System.Drawing.Point(861, 7);
            this.SelectDirectoryCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SelectDirectoryCtl.Name = "SelectDirectoryCtl";
            this.SelectDirectoryCtl.Size = new System.Drawing.Size(40, 28);
            this.SelectDirectoryCtl.TabIndex = 2;
            this.SelectDirectoryCtl.Text = "...";
            this.SelectDirectoryCtl.UseVisualStyleBackColor = true;
            this.SelectDirectoryCtl.Click += new System.EventHandler(this.SelectDirectoryCtl_Click);
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
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Destination Directory";
            // 
            // OutputFileCtl
            // 
            this.OutputFileCtl.AcceptsReturn = true;
            this.OutputFileCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputFileCtl.Location = new System.Drawing.Point(144, 40);
            this.OutputFileCtl.Margin = new System.Windows.Forms.Padding(4);
            this.OutputFileCtl.Name = "OutputFileCtl";
            this.OutputFileCtl.Size = new System.Drawing.Size(709, 22);
            this.OutputFileCtl.TabIndex = 4;
            // 
            // OutputDirectoryCtl
            // 
            this.OutputDirectoryCtl.AcceptsReturn = true;
            this.OutputDirectoryCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputDirectoryCtl.Location = new System.Drawing.Point(144, 10);
            this.OutputDirectoryCtl.Margin = new System.Windows.Forms.Padding(4);
            this.OutputDirectoryCtl.Name = "OutputDirectoryCtl";
            this.OutputDirectoryCtl.Size = new System.Drawing.Size(709, 22);
            this.OutputDirectoryCtl.TabIndex = 1;
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
        private System.Windows.Forms.Button SelectDirectoryCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox OutputDirectoryCtl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox OutputFileCtl;
        private System.Windows.Forms.Button DefineDestinationFileCtl;
    }
}
