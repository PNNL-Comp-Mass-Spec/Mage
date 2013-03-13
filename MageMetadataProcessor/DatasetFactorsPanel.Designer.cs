namespace MageMetadataProcessor {
    partial class DatasetFactorsPanel {
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
      this.GetResultsCrosstabBtn = new System.Windows.Forms.Button();
      this.GetMetadataBtn = new System.Windows.Forms.Button();
      this.GetFactorsBtn = new System.Windows.Forms.Button();
      this.FactorCountBtn = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.DatasetNameCtl = new System.Windows.Forms.TextBox();
      this.DataPackageNumberCtl = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.DataPackageNumberCtl);
      this.panel1.Controls.Add(this.GetResultsCrosstabBtn);
      this.panel1.Controls.Add(this.GetMetadataBtn);
      this.panel1.Controls.Add(this.GetFactorsBtn);
      this.panel1.Controls.Add(this.FactorCountBtn);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.DatasetNameCtl);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(5, 5);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(661, 93);
      this.panel1.TabIndex = 0;
      // 
      // GetResultsCrosstabBtn
      // 
      this.GetResultsCrosstabBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.GetResultsCrosstabBtn.Location = new System.Drawing.Point(485, 62);
      this.GetResultsCrosstabBtn.Name = "GetResultsCrosstabBtn";
      this.GetResultsCrosstabBtn.Size = new System.Drawing.Size(169, 23);
      this.GetResultsCrosstabBtn.TabIndex = 14;
      this.GetResultsCrosstabBtn.Text = "Get Dataset Factor Crosstab";
      this.GetResultsCrosstabBtn.UseVisualStyleBackColor = true;
      this.GetResultsCrosstabBtn.Click += new System.EventHandler(this.GetResultsCrosstabBtn_Click);
      // 
      // GetMetadataBtn
      // 
      this.GetMetadataBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.GetMetadataBtn.Location = new System.Drawing.Point(310, 62);
      this.GetMetadataBtn.Name = "GetMetadataBtn";
      this.GetMetadataBtn.Size = new System.Drawing.Size(169, 23);
      this.GetMetadataBtn.TabIndex = 13;
      this.GetMetadataBtn.Text = "Get Dataset Metadata";
      this.GetMetadataBtn.UseVisualStyleBackColor = true;
      this.GetMetadataBtn.Click += new System.EventHandler(this.GetMetadataBtn_Click);
      // 
      // GetFactorsBtn
      // 
      this.GetFactorsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.GetFactorsBtn.Location = new System.Drawing.Point(485, 36);
      this.GetFactorsBtn.Name = "GetFactorsBtn";
      this.GetFactorsBtn.Size = new System.Drawing.Size(169, 23);
      this.GetFactorsBtn.TabIndex = 12;
      this.GetFactorsBtn.Text = "Get Dataset Factor List";
      this.GetFactorsBtn.UseVisualStyleBackColor = true;
      this.GetFactorsBtn.Click += new System.EventHandler(this.GetFactorsBtn_Click);
      // 
      // FactorCountBtn
      // 
      this.FactorCountBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.FactorCountBtn.Location = new System.Drawing.Point(485, 10);
      this.FactorCountBtn.Name = "FactorCountBtn";
      this.FactorCountBtn.Size = new System.Drawing.Size(169, 23);
      this.FactorCountBtn.TabIndex = 11;
      this.FactorCountBtn.Text = "Get Dataset Factor Count";
      this.FactorCountBtn.UseVisualStyleBackColor = true;
      this.FactorCountBtn.Click += new System.EventHandler(this.FactorCountBtn_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(14, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(75, 13);
      this.label1.TabIndex = 10;
      this.label1.Text = "Dataset Name";
      // 
      // DatasetNameCtl
      // 
      this.DatasetNameCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DatasetNameCtl.Location = new System.Drawing.Point(100, 12);
      this.DatasetNameCtl.Name = "DatasetNameCtl";
      this.DatasetNameCtl.Size = new System.Drawing.Size(379, 20);
      this.DatasetNameCtl.TabIndex = 9;
      this.DatasetNameCtl.Text = "sarc_ms";
      // 
      // DataPackageNumberCtl
      // 
      this.DataPackageNumberCtl.Location = new System.Drawing.Point(100, 44);
      this.DataPackageNumberCtl.Name = "DataPackageNumberCtl";
      this.DataPackageNumberCtl.Size = new System.Drawing.Size(122, 20);
      this.DataPackageNumberCtl.TabIndex = 15;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(14, 47);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(76, 13);
      this.label2.TabIndex = 16;
      this.label2.Text = "Data Package";
      // 
      // DatasetFactorsPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.panel1);
      this.Name = "DatasetFactorsPanel";
      this.Padding = new System.Windows.Forms.Padding(5);
      this.Size = new System.Drawing.Size(671, 103);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button GetMetadataBtn;
        private System.Windows.Forms.Button GetFactorsBtn;
        private System.Windows.Forms.Button FactorCountBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DatasetNameCtl;
        private System.Windows.Forms.Button GetResultsCrosstabBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DataPackageNumberCtl;
    }
}
