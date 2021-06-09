namespace MageExtractorTest
{
    partial class Form1
    {
        // Ignore Spelling: Mage

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.TestRootPathCtl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gridViewDisplayControl1 = new MageDisplayLib.GridViewDisplayControl();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 231);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(667, 293);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(604, 530);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Selected";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(523, 530);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "All";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TestRootPathCtl
            // 
            this.TestRootPathCtl.Location = new System.Drawing.Point(105, 13);
            this.TestRootPathCtl.Name = "TestRootPathCtl";
            this.TestRootPathCtl.Size = new System.Drawing.Size(445, 20);
            this.TestRootPathCtl.TabIndex = 4;
            this.TestRootPathCtl.Text = "C:\\Data\\ExtractorTests";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Test Root Folder";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // gridViewDisplayControl1
            // 
            this.gridViewDisplayControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridViewDisplayControl1.HeaderVisible = true;
            this.gridViewDisplayControl1.ItemBlockSize = 25;
            this.gridViewDisplayControl1.Location = new System.Drawing.Point(12, 39);
            this.gridViewDisplayControl1.Name = "gridViewDisplayControl1";
            this.gridViewDisplayControl1.Notice = "";
            this.gridViewDisplayControl1.PageTitle = "Test Cases";
            this.gridViewDisplayControl1.Size = new System.Drawing.Size(667, 164);
            this.gridViewDisplayControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 565);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TestRootPathCtl);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.gridViewDisplayControl1);
            this.Name = "Form1";
            this.Text = "Mage Extractor Pipeline Tests";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MageDisplayLib.GridViewDisplayControl gridViewDisplayControl1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox TestRootPathCtl;
        private System.Windows.Forms.Label label1;
    }
}

