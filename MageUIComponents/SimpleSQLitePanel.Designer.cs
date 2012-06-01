namespace MageUIComponents {
    partial class SimpleSQLitePanel {
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
            this.BrowseForFileBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TableNameCtl = new System.Windows.Forms.TextBox();
            this.DBFilePathCtl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.BrowseForFileBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.TableNameCtl);
            this.panel1.Controls.Add(this.DBFilePathCtl);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(590, 30);
            this.panel1.TabIndex = 6;
            // 
            // BrowseForFileBtn
            // 
            this.BrowseForFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseForFileBtn.Location = new System.Drawing.Point(555, 2);
            this.BrowseForFileBtn.Name = "BrowseForFileBtn";
            this.BrowseForFileBtn.Size = new System.Drawing.Size(30, 23);
            this.BrowseForFileBtn.TabIndex = 5;
            this.BrowseForFileBtn.Text = "...";
            this.BrowseForFileBtn.UseVisualStyleBackColor = true;
            this.BrowseForFileBtn.Click += new System.EventHandler(this.BrowseForFileBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Table";
            // 
            // TableNameCtl
            // 
            this.TableNameCtl.Location = new System.Drawing.Point(43, 4);
            this.TableNameCtl.Name = "TableNameCtl";
            this.TableNameCtl.Size = new System.Drawing.Size(191, 20);
            this.TableNameCtl.TabIndex = 0;
            // 
            // DBFilePathCtl
            // 
            this.DBFilePathCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DBFilePathCtl.Location = new System.Drawing.Point(298, 4);
            this.DBFilePathCtl.Name = "DBFilePathCtl";
            this.DBFilePathCtl.Size = new System.Drawing.Size(251, 20);
            this.DBFilePathCtl.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(240, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database";
            // 
            // SimpleSQLitePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "SimpleSQLitePanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(600, 40);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BrowseForFileBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TableNameCtl;
        private System.Windows.Forms.TextBox DBFilePathCtl;
        private System.Windows.Forms.Label label2;
    }
}
