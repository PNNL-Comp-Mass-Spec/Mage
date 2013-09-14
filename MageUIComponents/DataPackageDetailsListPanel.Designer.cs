namespace MageUIComponents {
    partial class DataPackageDetailsListPanel {
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
			this.ItemListCtl = new System.Windows.Forms.TextBox();
			this.GetDatasetsCtl = new System.Windows.Forms.Button();
			this.LegendCtl = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.ItemListCtl);
			this.panel1.Controls.Add(this.GetDatasetsCtl);
			this.panel1.Controls.Add(this.LegendCtl);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(7, 6);
			this.panel1.Margin = new System.Windows.Forms.Padding(4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1136, 96);
			this.panel1.TabIndex = 1;
			// 
			// ItemListCtl
			// 
			this.ItemListCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ItemListCtl.Location = new System.Drawing.Point(12, 9);
			this.ItemListCtl.Margin = new System.Windows.Forms.Padding(4);
			this.ItemListCtl.Multiline = true;
			this.ItemListCtl.Name = "ItemListCtl";
			this.ItemListCtl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ItemListCtl.Size = new System.Drawing.Size(931, 81);
			this.ItemListCtl.TabIndex = 11;
			this.ItemListCtl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemListCtlKeyDown);
			this.ItemListCtl.Leave += new System.EventHandler(this.ItemListCtlLeave);
			// 
			// GetDatasetsCtl
			// 
			this.GetDatasetsCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.GetDatasetsCtl.Location = new System.Drawing.Point(951, 62);
			this.GetDatasetsCtl.Margin = new System.Windows.Forms.Padding(4);
			this.GetDatasetsCtl.Name = "GetDatasetsCtl";
			this.GetDatasetsCtl.Size = new System.Drawing.Size(179, 28);
			this.GetDatasetsCtl.TabIndex = 10;
			this.GetDatasetsCtl.Text = "&Get Data Packages";
			this.GetDatasetsCtl.UseVisualStyleBackColor = true;
			this.GetDatasetsCtl.Click += new System.EventHandler(this.ItemListCtlClick);
			// 
			// LegendCtl
			// 
			this.LegendCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LegendCtl.AutoSize = true;
			this.LegendCtl.Location = new System.Drawing.Point(951, 9);
			this.LegendCtl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LegendCtl.Name = "LegendCtl";
			this.LegendCtl.Size = new System.Drawing.Size(130, 17);
			this.LegendCtl.TabIndex = 9;
			this.LegendCtl.Text = "(Data package IDs)";
			this.LegendCtl.Click += new System.EventHandler(this.LegendCtlClick);
			// 
			// DataPackageDetailsListPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "DataPackageDetailsListPanel";
			this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
			this.Size = new System.Drawing.Size(1150, 108);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ItemListCtl;
		private System.Windows.Forms.Button GetDatasetsCtl;
        private System.Windows.Forms.Label LegendCtl;
    }
}
