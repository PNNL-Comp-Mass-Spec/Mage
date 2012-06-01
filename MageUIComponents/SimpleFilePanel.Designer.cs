namespace MageUIComponents {
    partial class SimpleFilePanel {
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
            this.SelectFileBtn = new System.Windows.Forms.Button();
            this.outputfilePathCtl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.SelectFileBtn);
            this.panel1.Controls.Add(this.outputfilePathCtl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(590, 30);
            this.panel1.TabIndex = 4;
            // 
            // SelectFileBtn
            // 
            this.SelectFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectFileBtn.Location = new System.Drawing.Point(555, 2);
            this.SelectFileBtn.Name = "SelectFileBtn";
            this.SelectFileBtn.Size = new System.Drawing.Size(30, 23);
            this.SelectFileBtn.TabIndex = 4;
            this.SelectFileBtn.Text = "...";
            this.SelectFileBtn.UseVisualStyleBackColor = true;
            this.SelectFileBtn.Click += new System.EventHandler(this.SelectFileBtn_Click);
            // 
            // outputfilePathCtl
            // 
            this.outputfilePathCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputfilePathCtl.Location = new System.Drawing.Point(86, 4);
            this.outputfilePathCtl.Name = "outputfilePathCtl";
            this.outputfilePathCtl.Size = new System.Drawing.Size(463, 20);
            this.outputfilePathCtl.TabIndex = 3;
            this.outputfilePathCtl.Text = "C:\\Data\\bork.txt";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Output File";
            // 
            // SimpleFilePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "SimpleFilePanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(600, 40);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button SelectFileBtn;
        private System.Windows.Forms.TextBox outputfilePathCtl;
        private System.Windows.Forms.Label label1;
    }
}
