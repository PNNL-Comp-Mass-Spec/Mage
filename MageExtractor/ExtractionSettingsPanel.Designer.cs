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
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Controls.Add(this.ExtractFromSelectedBtn);
            this.panel3.Controls.Add(this.ExtractFromAllBtn);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(5, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1039, 74);
            this.panel3.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ResultTypeNameCtl);
            this.groupBox3.Location = new System.Drawing.Point(6, 11);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 50);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Result Type To Extract";
            // 
            // ResultTypeNameCtl
            // 
            this.ResultTypeNameCtl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ResultTypeNameCtl.FormattingEnabled = true;
            this.ResultTypeNameCtl.Location = new System.Drawing.Point(12, 22);
            this.ResultTypeNameCtl.Name = "ResultTypeNameCtl";
            this.ResultTypeNameCtl.Size = new System.Drawing.Size(166, 21);
            this.ResultTypeNameCtl.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ClearFilterBtn);
            this.groupBox2.Controls.Add(this.SelectFilterBtn);
            this.groupBox2.Controls.Add(this.FilterSetIDCtl);
            this.groupBox2.Controls.Add(this.KeepResultsCtl);
            this.groupBox2.Location = new System.Drawing.Point(196, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(338, 50);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filter For Result Type";
            // 
            // ClearFilterBtn
            // 
            this.ClearFilterBtn.Location = new System.Drawing.Point(77, 20);
            this.ClearFilterBtn.Name = "ClearFilterBtn";
            this.ClearFilterBtn.Size = new System.Drawing.Size(75, 23);
            this.ClearFilterBtn.TabIndex = 19;
            this.ClearFilterBtn.Text = "Clear";
            this.ClearFilterBtn.UseVisualStyleBackColor = true;
            this.ClearFilterBtn.Click += new System.EventHandler(this.ClearFilterBtn_Click);
            // 
            // SelectFilterBtn
            // 
            this.SelectFilterBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectFilterBtn.Location = new System.Drawing.Point(153, 20);
            this.SelectFilterBtn.Name = "SelectFilterBtn";
            this.SelectFilterBtn.Size = new System.Drawing.Size(70, 23);
            this.SelectFilterBtn.TabIndex = 17;
            this.SelectFilterBtn.Text = "Select...";
            this.SelectFilterBtn.UseVisualStyleBackColor = true;
            this.SelectFilterBtn.Click += new System.EventHandler(this.SelectFilterBtn_Click);
            // 
            // FilterSetIDCtl
            // 
            this.FilterSetIDCtl.AutoSize = true;
            this.FilterSetIDCtl.Location = new System.Drawing.Point(28, 25);
            this.FilterSetIDCtl.Name = "FilterSetIDCtl";
            this.FilterSetIDCtl.Size = new System.Drawing.Size(44, 13);
            this.FilterSetIDCtl.TabIndex = 18;
            this.FilterSetIDCtl.Text = "All Pass";
            // 
            // KeepResultsCtl
            // 
            this.KeepResultsCtl.AutoSize = true;
            this.KeepResultsCtl.Location = new System.Drawing.Point(229, 24);
            this.KeepResultsCtl.Name = "KeepResultsCtl";
            this.KeepResultsCtl.Size = new System.Drawing.Size(103, 17);
            this.KeepResultsCtl.TabIndex = 6;
            this.KeepResultsCtl.Text = "Keep All Results";
            this.KeepResultsCtl.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.MSGFCutoffCtl);
            this.groupBox1.Location = new System.Drawing.Point(540, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(145, 50);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MSGF Cutoff";
            // 
            // MSGFCutoffCtl
            // 
            this.MSGFCutoffCtl.FormattingEnabled = true;
            this.MSGFCutoffCtl.Location = new System.Drawing.Point(18, 22);
            this.MSGFCutoffCtl.Name = "MSGFCutoffCtl";
            this.MSGFCutoffCtl.Size = new System.Drawing.Size(121, 21);
            this.MSGFCutoffCtl.TabIndex = 7;
            // 
            // ExtractFromSelectedBtn
            // 
            this.ExtractFromSelectedBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtractFromSelectedBtn.Location = new System.Drawing.Point(845, 7);
            this.ExtractFromSelectedBtn.Name = "ExtractFromSelectedBtn";
            this.ExtractFromSelectedBtn.Size = new System.Drawing.Size(186, 27);
            this.ExtractFromSelectedBtn.TabIndex = 0;
            this.ExtractFromSelectedBtn.Text = "Extract Results From &Selected Jobs";
            this.ExtractFromSelectedBtn.UseVisualStyleBackColor = true;
            this.ExtractFromSelectedBtn.Click += new System.EventHandler(this.ExtractFromSelectedBtn_Click);
            // 
            // ExtractFromAllBtn
            // 
            this.ExtractFromAllBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExtractFromAllBtn.Location = new System.Drawing.Point(845, 40);
            this.ExtractFromAllBtn.Name = "ExtractFromAllBtn";
            this.ExtractFromAllBtn.Size = new System.Drawing.Size(186, 27);
            this.ExtractFromAllBtn.TabIndex = 0;
            this.ExtractFromAllBtn.Text = "Extract Results From &All Jobs";
            this.ExtractFromAllBtn.UseVisualStyleBackColor = true;
            this.ExtractFromAllBtn.Click += new System.EventHandler(this.ExtractFromAllBtn_Click);
            // 
            // ExtractionSettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Name = "ExtractionSettingsPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(1049, 84);
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
