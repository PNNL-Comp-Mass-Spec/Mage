namespace MageUIComponents {
    partial class DatasetQueryPanel {
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.DatasetCtl = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.InstrumentCtl = new System.Windows.Forms.TextBox();
			this.StateCtl = new System.Windows.Forms.TextBox();
			this.TypeCtl = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.GetDatasetsCtl = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.tableLayoutPanel1);
			this.panel1.Controls.Add(this.GetDatasetsCtl);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(5, 5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(698, 100);
			this.panel1.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 94F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tableLayoutPanel1.Controls.Add(this.DatasetCtl, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label4, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.InstrumentCtl, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.StateCtl, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.TypeCtl, 3, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.81967F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.18033F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(693, 59);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// DatasetCtl
			// 
			this.DatasetCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DatasetCtl.Location = new System.Drawing.Point(67, 3);
			this.DatasetCtl.Name = "DatasetCtl";
			this.DatasetCtl.Size = new System.Drawing.Size(315, 20);
			this.DatasetCtl.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(445, 37);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(31, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Type";
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(444, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "State";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(5, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Instrument";
			// 
			// InstrumentCtl
			// 
			this.InstrumentCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.InstrumentCtl.Location = new System.Drawing.Point(67, 32);
			this.InstrumentCtl.Name = "InstrumentCtl";
			this.InstrumentCtl.Size = new System.Drawing.Size(315, 20);
			this.InstrumentCtl.TabIndex = 6;
			// 
			// StateCtl
			// 
			this.StateCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.StateCtl.Location = new System.Drawing.Point(482, 3);
			this.StateCtl.Name = "StateCtl";
			this.StateCtl.Size = new System.Drawing.Size(208, 20);
			this.StateCtl.TabIndex = 7;
			// 
			// TypeCtl
			// 
			this.TypeCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TypeCtl.Location = new System.Drawing.Point(482, 32);
			this.TypeCtl.Name = "TypeCtl";
			this.TypeCtl.Size = new System.Drawing.Size(208, 20);
			this.TypeCtl.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Dataset";
			// 
			// GetDatasetsCtl
			// 
			this.GetDatasetsCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.GetDatasetsCtl.Location = new System.Drawing.Point(601, 72);
			this.GetDatasetsCtl.Name = "GetDatasetsCtl";
			this.GetDatasetsCtl.Size = new System.Drawing.Size(92, 23);
			this.GetDatasetsCtl.TabIndex = 6;
			this.GetDatasetsCtl.Text = "&Get Datasets";
			this.GetDatasetsCtl.UseVisualStyleBackColor = true;
			this.GetDatasetsCtl.Click += new System.EventHandler(this.GetDatasetsCtl_Click);
			// 
			// DatasetQueryPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "DatasetQueryPanel";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.Size = new System.Drawing.Size(708, 110);
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox DatasetCtl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox InstrumentCtl;
        private System.Windows.Forms.TextBox StateCtl;
        private System.Windows.Forms.TextBox TypeCtl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GetDatasetsCtl;
    }
}
