namespace Ranger {
    partial class RangerForm {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.StatusCtl = new System.Windows.Forms.Label();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.incrementalParameterSubPanel1 = new MageUIComponents.IncrementalParameterSubPanel();
            this.incrementalParameterSubPanel2 = new MageUIComponents.IncrementalParameterSubPanel();
            this.incrementalParameterSubPanel3 = new MageUIComponents.IncrementalParameterSubPanel();
            this.incrementalParameterSubPanel4 = new MageUIComponents.IncrementalParameterSubPanel();
            this.incrementalParameterSubPanel5 = new MageUIComponents.IncrementalParameterSubPanel();
            this.incrementalParameterSubPanel6 = new MageUIComponents.IncrementalParameterSubPanel();
            this.simpleFilePanel1 = new MageUIComponents.SimpleFilePanel();
            this.simpleSQLitePanel1 = new MageUIComponents.SimpleSQLitePanel();
            this.incrementalParameterLitSubPanel1 = new MageUIComponents.IncrementalParameterLitSubPanel();
            this.incrementalParameterLitSubPanel2 = new MageUIComponents.IncrementalParameterLitSubPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterSubPanel1);
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterSubPanel2);
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterSubPanel3);
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterSubPanel4);
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterSubPanel5);
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterSubPanel6);
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterLitSubPanel1);
            this.flowLayoutPanel1.Controls.Add(this.incrementalParameterLitSubPanel2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(865, 302);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(5, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(855, 70);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.simpleFilePanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(847, 44);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Tag = "SaveToFile";
            this.tabPage1.Text = "Save to file";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.simpleSQLitePanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(847, 44);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Tag = "SaveToSQLite";
            this.tabPage2.Text = "Save to SQLite";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.StatusCtl);
            this.panel1.Controls.Add(this.SaveBtn);
            this.panel1.Location = new System.Drawing.Point(5, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(853, 30);
            this.panel1.TabIndex = 5;
            // 
            // StatusCtl
            // 
            this.StatusCtl.AutoSize = true;
            this.StatusCtl.Location = new System.Drawing.Point(3, 8);
            this.StatusCtl.Name = "StatusCtl";
            this.StatusCtl.Size = new System.Drawing.Size(41, 13);
            this.StatusCtl.TabIndex = 1;
            this.StatusCtl.Text = "(status)";
            // 
            // SaveBtn
            // 
            this.SaveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveBtn.Location = new System.Drawing.Point(771, 3);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveBtn.TabIndex = 0;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 302);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(865, 107);
            this.panel2.TabIndex = 6;
            // 
            // incrementalParameterSubPanel1
            // 
            this.incrementalParameterSubPanel1.Active = true;
            this.incrementalParameterSubPanel1.Increment = "";
            this.incrementalParameterSubPanel1.Location = new System.Drawing.Point(3, 3);
            this.incrementalParameterSubPanel1.Lower = "";
            this.incrementalParameterSubPanel1.Name = "incrementalParameterSubPanel1";
            this.incrementalParameterSubPanel1.Operator = "=";
            this.incrementalParameterSubPanel1.ParamName = "";
            this.incrementalParameterSubPanel1.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterSubPanel1.TabIndex = 0;
            this.incrementalParameterSubPanel1.Upper = "";
            // 
            // incrementalParameterSubPanel2
            // 
            this.incrementalParameterSubPanel2.Active = true;
            this.incrementalParameterSubPanel2.Increment = "";
            this.incrementalParameterSubPanel2.Location = new System.Drawing.Point(3, 39);
            this.incrementalParameterSubPanel2.Lower = "";
            this.incrementalParameterSubPanel2.Name = "incrementalParameterSubPanel2";
            this.incrementalParameterSubPanel2.Operator = "=";
            this.incrementalParameterSubPanel2.ParamName = "";
            this.incrementalParameterSubPanel2.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterSubPanel2.TabIndex = 1;
            this.incrementalParameterSubPanel2.Upper = "";
            // 
            // incrementalParameterSubPanel3
            // 
            this.incrementalParameterSubPanel3.Active = true;
            this.incrementalParameterSubPanel3.Increment = "";
            this.incrementalParameterSubPanel3.Location = new System.Drawing.Point(3, 75);
            this.incrementalParameterSubPanel3.Lower = "";
            this.incrementalParameterSubPanel3.Name = "incrementalParameterSubPanel3";
            this.incrementalParameterSubPanel3.Operator = "=";
            this.incrementalParameterSubPanel3.ParamName = "";
            this.incrementalParameterSubPanel3.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterSubPanel3.TabIndex = 2;
            this.incrementalParameterSubPanel3.Upper = "";
            // 
            // incrementalParameterSubPanel4
            // 
            this.incrementalParameterSubPanel4.Active = true;
            this.incrementalParameterSubPanel4.Increment = "";
            this.incrementalParameterSubPanel4.Location = new System.Drawing.Point(3, 111);
            this.incrementalParameterSubPanel4.Lower = "";
            this.incrementalParameterSubPanel4.Name = "incrementalParameterSubPanel4";
            this.incrementalParameterSubPanel4.Operator = "=";
            this.incrementalParameterSubPanel4.ParamName = "";
            this.incrementalParameterSubPanel4.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterSubPanel4.TabIndex = 3;
            this.incrementalParameterSubPanel4.Upper = "";
            // 
            // incrementalParameterSubPanel5
            // 
            this.incrementalParameterSubPanel5.Active = true;
            this.incrementalParameterSubPanel5.Increment = "";
            this.incrementalParameterSubPanel5.Location = new System.Drawing.Point(3, 147);
            this.incrementalParameterSubPanel5.Lower = "";
            this.incrementalParameterSubPanel5.Name = "incrementalParameterSubPanel5";
            this.incrementalParameterSubPanel5.Operator = "=";
            this.incrementalParameterSubPanel5.ParamName = "";
            this.incrementalParameterSubPanel5.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterSubPanel5.TabIndex = 4;
            this.incrementalParameterSubPanel5.Upper = "";
            // 
            // incrementalParameterSubPanel6
            // 
            this.incrementalParameterSubPanel6.Active = true;
            this.incrementalParameterSubPanel6.Increment = "";
            this.incrementalParameterSubPanel6.Location = new System.Drawing.Point(3, 183);
            this.incrementalParameterSubPanel6.Lower = "";
            this.incrementalParameterSubPanel6.Name = "incrementalParameterSubPanel6";
            this.incrementalParameterSubPanel6.Operator = "=";
            this.incrementalParameterSubPanel6.ParamName = "";
            this.incrementalParameterSubPanel6.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterSubPanel6.TabIndex = 5;
            this.incrementalParameterSubPanel6.Upper = "";
            // 
            // simpleFilePanel1
            // 
            this.simpleFilePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleFilePanel1.FilePath = "C:\\Data\\test.txt";
            this.simpleFilePanel1.Location = new System.Drawing.Point(3, 3);
            this.simpleFilePanel1.Margin = new System.Windows.Forms.Padding(0);
            this.simpleFilePanel1.Name = "simpleFilePanel1";
            this.simpleFilePanel1.Padding = new System.Windows.Forms.Padding(5);
            this.simpleFilePanel1.Size = new System.Drawing.Size(841, 38);
            this.simpleFilePanel1.TabIndex = 0;
            // 
            // simpleSQLitePanel1
            // 
            this.simpleSQLitePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleSQLitePanel1.Location = new System.Drawing.Point(3, 3);
            this.simpleSQLitePanel1.Margin = new System.Windows.Forms.Padding(0);
            this.simpleSQLitePanel1.Name = "simpleSQLitePanel1";
            this.simpleSQLitePanel1.Padding = new System.Windows.Forms.Padding(5);
            this.simpleSQLitePanel1.Size = new System.Drawing.Size(841, 38);
            this.simpleSQLitePanel1.TabIndex = 0;
            // 
            // incrementalParameterLitSubPanel1
            // 
            this.incrementalParameterLitSubPanel1.Location = new System.Drawing.Point(3, 219);
            this.incrementalParameterLitSubPanel1.Name = "incrementalParameterLitSubPanel1";
            this.incrementalParameterLitSubPanel1.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterLitSubPanel1.TabIndex = 6;
            // 
            // incrementalParameterLitSubPanel2
            // 
            this.incrementalParameterLitSubPanel2.Location = new System.Drawing.Point(3, 255);
            this.incrementalParameterLitSubPanel2.Name = "incrementalParameterLitSubPanel2";
            this.incrementalParameterLitSubPanel2.Size = new System.Drawing.Size(820, 30);
            this.incrementalParameterLitSubPanel2.TabIndex = 7;
            // 
            // RangerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 409);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Name = "RangerForm";
            this.Text = "Ranger 1.4 (2011-10-21)";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private MageUIComponents.IncrementalParameterSubPanel incrementalParameterSubPanel1;
        private MageUIComponents.IncrementalParameterSubPanel incrementalParameterSubPanel2;
        private MageUIComponents.IncrementalParameterSubPanel incrementalParameterSubPanel3;
        private MageUIComponents.IncrementalParameterSubPanel incrementalParameterSubPanel4;
        private MageUIComponents.IncrementalParameterSubPanel incrementalParameterSubPanel5;
        private MageUIComponents.IncrementalParameterSubPanel incrementalParameterSubPanel6;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button SaveBtn;
        private MageUIComponents.SimpleFilePanel simpleFilePanel1;
        private MageUIComponents.SimpleSQLitePanel simpleSQLitePanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label StatusCtl;
        private MageUIComponents.IncrementalParameterLitSubPanel incrementalParameterLitSubPanel1;
        private MageUIComponents.IncrementalParameterLitSubPanel incrementalParameterLitSubPanel2;

    }
}

