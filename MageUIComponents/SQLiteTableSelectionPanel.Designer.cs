namespace MageUIComponents
{
    partial class SQLiteTableSelectionPanel
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.DatabasePathCtl = new System.Windows.Forms.Label();
            this.TableNameCtl = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CancelCtl = new System.Windows.Forms.Button();
            this.OkCtl = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TableListCtl = new MageDisplayLib.GridViewDisplayControl();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.TableListCtl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 50);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(394, 275);
            this.panel1.TabIndex = 0;
            // 
            // DatabasePathCtl
            // 
            this.DatabasePathCtl.AutoSize = true;
            this.DatabasePathCtl.Location = new System.Drawing.Point(75, 6);
            this.DatabasePathCtl.Name = "DatabasePathCtl";
            this.DatabasePathCtl.Size = new System.Drawing.Size(57, 13);
            this.DatabasePathCtl.TabIndex = 4;
            this.DatabasePathCtl.Text = "(database)";
            // 
            // TableNameCtl
            // 
            this.TableNameCtl.AutoSize = true;
            this.TableNameCtl.Location = new System.Drawing.Point(75, 25);
            this.TableNameCtl.Name = "TableNameCtl";
            this.TableNameCtl.Size = new System.Drawing.Size(36, 13);
            this.TableNameCtl.TabIndex = 4;
            this.TableNameCtl.Text = "(table)";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CancelCtl);
            this.panel2.Controls.Add(this.OkCtl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 325);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(394, 37);
            this.panel2.TabIndex = 5;
            // 
            // CancelCtl
            // 
            this.CancelCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelCtl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelCtl.Location = new System.Drawing.Point(233, 8);
            this.CancelCtl.Name = "CancelCtl";
            this.CancelCtl.Size = new System.Drawing.Size(75, 23);
            this.CancelCtl.TabIndex = 1;
            this.CancelCtl.Text = "Cancel";
            this.CancelCtl.UseVisualStyleBackColor = true;
            // 
            // OkCtl
            // 
            this.OkCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OkCtl.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkCtl.Location = new System.Drawing.Point(314, 8);
            this.OkCtl.Name = "OkCtl";
            this.OkCtl.Size = new System.Drawing.Size(75, 23);
            this.OkCtl.TabIndex = 0;
            this.OkCtl.Text = "OK";
            this.OkCtl.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.TableNameCtl);
            this.panel3.Controls.Add(this.DatabasePathCtl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(394, 50);
            this.panel3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Table";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Database:";
            // 
            // TableListCtl
            // 
            this.TableListCtl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableListCtl.HeaderVisible = true;
            this.TableListCtl.ItemBlockSize = 25;
            this.TableListCtl.Location = new System.Drawing.Point(5, 5);
            this.TableListCtl.Name = "TableListCtl";
            this.TableListCtl.Notice = "";
            this.TableListCtl.PageTitle = "Title";
            this.TableListCtl.Size = new System.Drawing.Size(384, 265);
            this.TableListCtl.TabIndex = 0;
            this.TableListCtl.SelectionChanged += new System.EventHandler<System.EventArgs>(this.TableListCtl_SelectionChanged);
            // 
            // SQLiteTableSelectionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 362);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "SQLiteTableSelectionPanel";
            this.Text = "Select Table";
            this.Load += new System.EventHandler(this.SQLiteTableSelectionPanel_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label DatabasePathCtl;
        private System.Windows.Forms.Label TableNameCtl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button CancelCtl;
        private System.Windows.Forms.Button OkCtl;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private MageDisplayLib.GridViewDisplayControl TableListCtl;
    }
}