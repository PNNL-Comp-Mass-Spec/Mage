namespace MageUIComponents
{
    partial class SQLiteDestinationPanel
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
            this.DefineSqLiteTableCtl = new System.Windows.Forms.Button();
            this.SelectSQLiteDbCtl = new System.Windows.Forms.Button();
            this.TableNameCtl = new System.Windows.Forms.TextBox();
            this.DatabaseNameCtl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.DefineSqLiteTableCtl);
            this.panel1.Controls.Add(this.SelectSQLiteDbCtl);
            this.panel1.Controls.Add(this.TableNameCtl);
            this.panel1.Controls.Add(this.DatabaseNameCtl);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(7, 6);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.panel1.Size = new System.Drawing.Size(907, 72);
            this.panel1.TabIndex = 1;
            // 
            // DefineSqLiteTableCtl
            // 
            this.DefineSqLiteTableCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefineSqLiteTableCtl.Location = new System.Drawing.Point(861, 37);
            this.DefineSqLiteTableCtl.Margin = new System.Windows.Forms.Padding(4);
            this.DefineSqLiteTableCtl.Name = "DefineSqLiteTableCtl";
            this.DefineSqLiteTableCtl.Size = new System.Drawing.Size(40, 28);
            this.DefineSqLiteTableCtl.TabIndex = 7;
            this.DefineSqLiteTableCtl.Text = "...";
            this.DefineSqLiteTableCtl.UseVisualStyleBackColor = true;
            this.DefineSqLiteTableCtl.Click += new System.EventHandler(this.DefineSqLiteTableCtl_Click);
            // 
            // SelectSQLiteDbCtl
            // 
            this.SelectSQLiteDbCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectSQLiteDbCtl.Location = new System.Drawing.Point(861, 7);
            this.SelectSQLiteDbCtl.Margin = new System.Windows.Forms.Padding(4);
            this.SelectSQLiteDbCtl.Name = "SelectSQLiteDbCtl";
            this.SelectSQLiteDbCtl.Size = new System.Drawing.Size(40, 28);
            this.SelectSQLiteDbCtl.TabIndex = 6;
            this.SelectSQLiteDbCtl.Text = "...";
            this.SelectSQLiteDbCtl.UseVisualStyleBackColor = true;
            this.SelectSQLiteDbCtl.Click += new System.EventHandler(this.SelectSqLiteDbCtl_Click);
            // 
            // TableNameCtl
            // 
            this.TableNameCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableNameCtl.Location = new System.Drawing.Point(133, 40);
            this.TableNameCtl.Margin = new System.Windows.Forms.Padding(4);
            this.TableNameCtl.Name = "TableNameCtl";
            this.TableNameCtl.Size = new System.Drawing.Size(720, 22);
            this.TableNameCtl.TabIndex = 4;
            // 
            // DatabaseNameCtl
            // 
            this.DatabaseNameCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DatabaseNameCtl.Location = new System.Drawing.Point(133, 10);
            this.DatabaseNameCtl.Margin = new System.Windows.Forms.Padding(4);
            this.DatabaseNameCtl.Name = "DatabaseNameCtl";
            this.DatabaseNameCtl.Size = new System.Drawing.Size(720, 22);
            this.DatabaseNameCtl.TabIndex = 3;
            this.DatabaseNameCtl.Validating += new System.ComponentModel.CancelEventHandler(this.ValidateSQLiteDBPath);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Table Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "SQLite Database";
            // 
            // SQLiteDestinationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SQLiteDestinationPanel";
            this.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Size = new System.Drawing.Size(921, 84);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button DefineSqLiteTableCtl;
        private System.Windows.Forms.Button SelectSQLiteDbCtl;
        private System.Windows.Forms.TextBox TableNameCtl;
        private System.Windows.Forms.TextBox DatabaseNameCtl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;




    }
}
