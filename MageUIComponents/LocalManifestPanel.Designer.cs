namespace MageUIComponents {
    partial class LocalManifestPanel {
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
            this.DefineManifestFileCtl = new System.Windows.Forms.Button();
            this.LocalManifestFileCtl = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.GetFilesCtl = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.DefineManifestFileCtl);
            this.panel1.Controls.Add(this.LocalManifestFileCtl);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.GetFilesCtl);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(748, 70);
            this.panel1.TabIndex = 7;
            // 
            // DefineManifestFileCtl
            // 
            this.DefineManifestFileCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefineManifestFileCtl.Location = new System.Drawing.Point(702, 9);
            this.DefineManifestFileCtl.Name = "DefineManifestFileCtl";
            this.DefineManifestFileCtl.Size = new System.Drawing.Size(30, 23);
            this.DefineManifestFileCtl.TabIndex = 5;
            this.DefineManifestFileCtl.Text = "...";
            this.DefineManifestFileCtl.UseVisualStyleBackColor = true;
            this.DefineManifestFileCtl.Click += new System.EventHandler(this.DefineManifestFileCtl_Click);
            // 
            // LocalManifestFileCtl
            // 
            this.LocalManifestFileCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalManifestFileCtl.Location = new System.Drawing.Point(90, 11);
            this.LocalManifestFileCtl.Name = "LocalManifestFileCtl";
            this.LocalManifestFileCtl.Size = new System.Drawing.Size(606, 20);
            this.LocalManifestFileCtl.TabIndex = 4;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(75, -26);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(695, 20);
            this.textBox3.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Manifiest File";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, -73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Input File";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, -47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Output File";
            // 
            // GetFilesCtl
            // 
            this.GetFilesCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetFilesCtl.Location = new System.Drawing.Point(587, 35);
            this.GetFilesCtl.Name = "GetFilesCtl";
            this.GetFilesCtl.Size = new System.Drawing.Size(145, 23);
            this.GetFilesCtl.TabIndex = 1;
            this.GetFilesCtl.Text = "Get Files";
            this.GetFilesCtl.UseVisualStyleBackColor = true;
            this.GetFilesCtl.Click += new System.EventHandler(this.GetFilesCtl_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(75, -50);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(695, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "C:\\Data\\test.txt";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, -23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Filter Spec";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(75, -76);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(695, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "C:\\Data\\syn\\Job_558940_sarc_OHSUdeptest_Glo_14_6Jan10_Doc_09-09-19_syn.txt";
            // 
            // LocalManifestPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "LocalManifestPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(758, 80);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button DefineManifestFileCtl;
        private System.Windows.Forms.TextBox LocalManifestFileCtl;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button GetFilesCtl;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;

    }
}
