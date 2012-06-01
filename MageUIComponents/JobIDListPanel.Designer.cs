namespace MageUIComponents {
    partial class JobIDListPanel {
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
            this.JobListCtl = new System.Windows.Forms.TextBox();
            this.GetJobsCtl = new System.Windows.Forms.Button();
            this.LegendCtl = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.JobListCtl);
            this.panel1.Controls.Add(this.GetJobsCtl);
            this.panel1.Controls.Add(this.LegendCtl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(624, 110);
            this.panel1.TabIndex = 0;
            // 
            // JobListCtl
            // 
            this.JobListCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.JobListCtl.Location = new System.Drawing.Point(9, 7);
            this.JobListCtl.Multiline = true;
            this.JobListCtl.Name = "JobListCtl";
            this.JobListCtl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.JobListCtl.Size = new System.Drawing.Size(600, 69);
            this.JobListCtl.TabIndex = 11;
            this.JobListCtl.Leave += new System.EventHandler(this.JobListCtl_Leave);
            this.JobListCtl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JobListCtl_KeyDown);
            
            // 
            // GetJobsCtl
            // 
            this.GetJobsCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.GetJobsCtl.Location = new System.Drawing.Point(543, 82);
            this.GetJobsCtl.Name = "GetJobsCtl";
            this.GetJobsCtl.Size = new System.Drawing.Size(76, 23);
            this.GetJobsCtl.TabIndex = 10;
            this.GetJobsCtl.Text = "&Get Jobs";
            this.GetJobsCtl.UseVisualStyleBackColor = true;
            this.GetJobsCtl.Click += new System.EventHandler(this.GetJobsCtl_Click);
            // 
            // LegendCtl
            // 
            this.LegendCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LegendCtl.AutoSize = true;
            this.LegendCtl.Location = new System.Drawing.Point(9, 87);
            this.LegendCtl.Name = "LegendCtl";
            this.LegendCtl.Size = new System.Drawing.Size(114, 13);
            this.LegendCtl.TabIndex = 9;
            this.LegendCtl.Text = "(Paste in list of job IDs)";
            // 
            // JobIDListPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "JobIDListPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(634, 120);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox JobListCtl;
        private System.Windows.Forms.Button GetJobsCtl;
        private System.Windows.Forms.Label LegendCtl;
    }
}
