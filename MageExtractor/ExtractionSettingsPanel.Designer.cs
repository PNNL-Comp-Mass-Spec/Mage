namespace MageExtractor {
    partial class ExtractionSettingsPanel {
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
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.ResultTypeNameCtl = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.ClearFilterBtn = new System.Windows.Forms.Button();
			this.SelectFilterBtn = new System.Windows.Forms.Button();
			this.FilterSetIDCtl = new System.Windows.Forms.Label();
			this.KeepResultsCtl = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.MSGFCutoffCtl = new System.Windows.Forms.ComboBox();
			this.ExtractFromSelectedBtn = new System.Windows.Forms.Button();
			this.ExtractFromAllBtn = new System.Windows.Forms.Button();
			this.panel3.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel3
			// 
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel3.Controls.Add(this.ExtractFromSelectedBtn);
			this.panel3.Controls.Add(this.ExtractFromAllBtn);
			this.panel3.Controls.Add(this.groupBox3);
			this.panel3.Controls.Add(this.groupBox2);
			this.panel3.Controls.Add(this.groupBox1);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(7, 6);
			this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(1193, 91);
			this.panel3.TabIndex = 0;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.ResultTypeNameCtl);
			this.groupBox3.Location = new System.Drawing.Point(8, 14);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox3.Size = new System.Drawing.Size(245, 62);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Result Type To Extract";
			// 
			// ResultTypeNameCtl
			// 
			this.ResultTypeNameCtl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ResultTypeNameCtl.FormattingEnabled = true;
			this.ResultTypeNameCtl.Location = new System.Drawing.Point(16, 27);
			this.ResultTypeNameCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.ResultTypeNameCtl.Name = "ResultTypeNameCtl";
			this.ResultTypeNameCtl.Size = new System.Drawing.Size(220, 24);
			this.ResultTypeNameCtl.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.ClearFilterBtn);
			this.groupBox2.Controls.Add(this.SelectFilterBtn);
			this.groupBox2.Controls.Add(this.FilterSetIDCtl);
			this.groupBox2.Controls.Add(this.KeepResultsCtl);
			this.groupBox2.Location = new System.Drawing.Point(261, 14);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox2.Size = new System.Drawing.Size(451, 62);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Filter For Result Type";
			// 
			// ClearFilterBtn
			// 
			this.ClearFilterBtn.Location = new System.Drawing.Point(103, 25);
			this.ClearFilterBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.ClearFilterBtn.Name = "ClearFilterBtn";
			this.ClearFilterBtn.Size = new System.Drawing.Size(100, 28);
			this.ClearFilterBtn.TabIndex = 1;
			this.ClearFilterBtn.Text = "Clear";
			this.ClearFilterBtn.UseVisualStyleBackColor = true;
			this.ClearFilterBtn.Click += new System.EventHandler(this.ClearFilterBtn_Click);
			// 
			// SelectFilterBtn
			// 
			this.SelectFilterBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SelectFilterBtn.Location = new System.Drawing.Point(204, 25);
			this.SelectFilterBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.SelectFilterBtn.Name = "SelectFilterBtn";
			this.SelectFilterBtn.Size = new System.Drawing.Size(93, 28);
			this.SelectFilterBtn.TabIndex = 2;
			this.SelectFilterBtn.Text = "Select...";
			this.SelectFilterBtn.UseVisualStyleBackColor = true;
			this.SelectFilterBtn.Click += new System.EventHandler(this.SelectFilterBtn_Click);
			// 
			// FilterSetIDCtl
			// 
			this.FilterSetIDCtl.AutoSize = true;
			this.FilterSetIDCtl.Location = new System.Drawing.Point(37, 31);
			this.FilterSetIDCtl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.FilterSetIDCtl.Name = "FilterSetIDCtl";
			this.FilterSetIDCtl.Size = new System.Drawing.Size(58, 17);
			this.FilterSetIDCtl.TabIndex = 0;
			this.FilterSetIDCtl.Text = "All Pass";
			// 
			// KeepResultsCtl
			// 
			this.KeepResultsCtl.AutoSize = true;
			this.KeepResultsCtl.Location = new System.Drawing.Point(305, 30);
			this.KeepResultsCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.KeepResultsCtl.Name = "KeepResultsCtl";
			this.KeepResultsCtl.Size = new System.Drawing.Size(133, 21);
			this.KeepResultsCtl.TabIndex = 3;
			this.KeepResultsCtl.Text = "Keep All Results";
			this.KeepResultsCtl.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.MSGFCutoffCtl);
			this.groupBox1.Location = new System.Drawing.Point(720, 14);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox1.Size = new System.Drawing.Size(193, 62);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "MSGF Cutoff";
			// 
			// MSGFCutoffCtl
			// 
			this.MSGFCutoffCtl.FormattingEnabled = true;
			this.MSGFCutoffCtl.Location = new System.Drawing.Point(24, 27);
			this.MSGFCutoffCtl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MSGFCutoffCtl.Name = "MSGFCutoffCtl";
			this.MSGFCutoffCtl.Size = new System.Drawing.Size(160, 24);
			this.MSGFCutoffCtl.TabIndex = 0;
			// 
			// ExtractFromSelectedBtn
			// 
			this.ExtractFromSelectedBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExtractFromSelectedBtn.Location = new System.Drawing.Point(935, 9);
			this.ExtractFromSelectedBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.ExtractFromSelectedBtn.Name = "ExtractFromSelectedBtn";
			this.ExtractFromSelectedBtn.Size = new System.Drawing.Size(248, 33);
			this.ExtractFromSelectedBtn.TabIndex = 3;
			this.ExtractFromSelectedBtn.Text = "Extract Results From &Selected Jobs";
			this.ExtractFromSelectedBtn.UseVisualStyleBackColor = true;
			this.ExtractFromSelectedBtn.Click += new System.EventHandler(this.ExtractFromSelectedBtn_Click);
			// 
			// ExtractFromAllBtn
			// 
			this.ExtractFromAllBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExtractFromAllBtn.Location = new System.Drawing.Point(935, 49);
			this.ExtractFromAllBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.ExtractFromAllBtn.Name = "ExtractFromAllBtn";
			this.ExtractFromAllBtn.Size = new System.Drawing.Size(248, 33);
			this.ExtractFromAllBtn.TabIndex = 4;
			this.ExtractFromAllBtn.Text = "Extract Results From &All Jobs";
			this.ExtractFromAllBtn.UseVisualStyleBackColor = true;
			this.ExtractFromAllBtn.Click += new System.EventHandler(this.ExtractFromAllBtn_Click);
			// 
			// ExtractionSettingsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel3);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ExtractionSettingsPanel";
			this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
			this.Size = new System.Drawing.Size(1207, 103);
			this.panel3.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox ResultTypeNameCtl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox KeepResultsCtl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox MSGFCutoffCtl;
        private System.Windows.Forms.Button ExtractFromSelectedBtn;
        private System.Windows.Forms.Button ExtractFromAllBtn;
        private System.Windows.Forms.Button SelectFilterBtn;
        private System.Windows.Forms.Label FilterSetIDCtl;
        private System.Windows.Forms.Button ClearFilterBtn;
    }
}
