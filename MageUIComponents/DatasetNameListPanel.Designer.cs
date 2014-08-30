namespace MageUIComponents
{
	partial class DatasetNameListPanel
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
			this.GetDatasetsCtl = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.DatasetListCtl = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.GetDatasetsCtl);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.DatasetListCtl);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(7, 6);
			this.panel1.Margin = new System.Windows.Forms.Padding(4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1082, 109);
			this.panel1.TabIndex = 1;
			// 
			// GetDatasetsCtl
			// 
			this.GetDatasetsCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.GetDatasetsCtl.Location = new System.Drawing.Point(935, 75);
			this.GetDatasetsCtl.Margin = new System.Windows.Forms.Padding(4);
			this.GetDatasetsCtl.Name = "GetDatasetsCtl";
			this.GetDatasetsCtl.Size = new System.Drawing.Size(141, 28);
			this.GetDatasetsCtl.TabIndex = 10;
			this.GetDatasetsCtl.Text = "&Get Datasets";
			this.GetDatasetsCtl.UseVisualStyleBackColor = true;
			this.GetDatasetsCtl.Click += new System.EventHandler(this.GetDatasetsCtl_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(932, 9);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 17);
			this.label1.TabIndex = 12;
			this.label1.Text = "(Dataset Names)";
			// 
			// DatasetListCtl
			// 
			this.DatasetListCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DatasetListCtl.Location = new System.Drawing.Point(12, 9);
			this.DatasetListCtl.Margin = new System.Windows.Forms.Padding(4);
			this.DatasetListCtl.Multiline = true;
			this.DatasetListCtl.Name = "DatasetListCtl";
			this.DatasetListCtl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.DatasetListCtl.Size = new System.Drawing.Size(912, 94);
			this.DatasetListCtl.TabIndex = 11;
			this.DatasetListCtl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DatasetListCtl_KeyDown);
			this.DatasetListCtl.Leave += new System.EventHandler(this.DatasetListCtl_Leave);
			// 
			// DatasetNameListPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "DatasetNameListPanel";
			this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
			this.Size = new System.Drawing.Size(1096, 121);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox DatasetListCtl;
		private System.Windows.Forms.Button GetDatasetsCtl;
		private System.Windows.Forms.Label label1;
	}
}
