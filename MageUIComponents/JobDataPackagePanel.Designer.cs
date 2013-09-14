namespace MageUIComponents {
    partial class JobDataPackagePanel {
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
			this.GetDatasetsCtl = new System.Windows.Forms.Button();
			this.DataPackageIDCtl = new System.Windows.Forms.TextBox();
			this.GetJobsCtl = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.GetDatasetsCtl);
			this.panel1.Controls.Add(this.DataPackageIDCtl);
			this.panel1.Controls.Add(this.GetJobsCtl);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(7, 6);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(653, 78);
			this.panel1.TabIndex = 1;
			// 
			// GetDatasetsCtl
			// 
			this.GetDatasetsCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.GetDatasetsCtl.Location = new System.Drawing.Point(496, 8);
			this.GetDatasetsCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.GetDatasetsCtl.Name = "GetDatasetsCtl";
			this.GetDatasetsCtl.Size = new System.Drawing.Size(151, 28);
			this.GetDatasetsCtl.TabIndex = 12;
			this.GetDatasetsCtl.Text = "&Get Datasets";
			this.GetDatasetsCtl.UseVisualStyleBackColor = true;
			this.GetDatasetsCtl.Click += new System.EventHandler(this.GetDatasetsCtl_Click);
			// 
			// DataPackageIDCtl
			// 
			this.DataPackageIDCtl.Location = new System.Drawing.Point(140, 9);
			this.DataPackageIDCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.DataPackageIDCtl.Name = "DataPackageIDCtl";
			this.DataPackageIDCtl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.DataPackageIDCtl.Size = new System.Drawing.Size(207, 22);
			this.DataPackageIDCtl.TabIndex = 11;
			// 
			// GetJobsCtl
			// 
			this.GetJobsCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.GetJobsCtl.Location = new System.Drawing.Point(496, 44);
			this.GetJobsCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.GetJobsCtl.Name = "GetJobsCtl";
			this.GetJobsCtl.Size = new System.Drawing.Size(151, 28);
			this.GetJobsCtl.TabIndex = 10;
			this.GetJobsCtl.Text = "&Get Jobs";
			this.GetJobsCtl.UseVisualStyleBackColor = true;
			this.GetJobsCtl.Click += new System.EventHandler(this.GetJobsCtl_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(4, 12);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(114, 17);
			this.label5.TabIndex = 9;
			this.label5.Text = "Data Package ID";
			// 
			// JobDataPackagePanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "JobDataPackagePanel";
			this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
			this.Size = new System.Drawing.Size(667, 90);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox DataPackageIDCtl;
        private System.Windows.Forms.Button GetJobsCtl;
        private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button GetDatasetsCtl;
    }
}
