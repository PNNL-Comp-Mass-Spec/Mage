namespace MageMetadataProcessor
{
    partial class RawQueryPanel
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
            this.RawSQLCtl = new System.Windows.Forms.TextBox();
            this.GetResultsBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.RawSQLCtl);
            this.panel1.Controls.Add(this.GetResultsBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(879, 98);
            this.panel1.TabIndex = 1;
            // 
            // RawSQLCtl
            // 
            this.RawSQLCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RawSQLCtl.Location = new System.Drawing.Point(7, 7);
            this.RawSQLCtl.Multiline = true;
            this.RawSQLCtl.Name = "RawSQLCtl";
            this.RawSQLCtl.Size = new System.Drawing.Size(705, 83);
            this.RawSQLCtl.TabIndex = 2;
            // 
            // GetResultsBtn
            // 
            this.GetResultsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GetResultsBtn.Location = new System.Drawing.Point(718, 67);
            this.GetResultsBtn.Name = "GetResultsBtn";
            this.GetResultsBtn.Size = new System.Drawing.Size(147, 23);
            this.GetResultsBtn.TabIndex = 0;
            this.GetResultsBtn.Text = "Get Results";
            this.GetResultsBtn.UseVisualStyleBackColor = true;
            this.GetResultsBtn.Click += new System.EventHandler(this.GetResultsBtn_Click);
            // 
            // RawQueryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "RawQueryPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(889, 108);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox RawSQLCtl;
        private System.Windows.Forms.Button GetResultsBtn;

    }
}
