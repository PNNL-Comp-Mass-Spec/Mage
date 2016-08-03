namespace MageDisplayLib
{
    partial class GridViewDisplayControl
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
            var dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            var dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkShiftClickMode = new System.Windows.Forms.CheckBox();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.lblNotice = new System.Windows.Forms.Label();
            this.gvQueryResults = new MageDisplayLib.GridViewDisplayControl.MyDataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvQueryResults)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkShiftClickMode);
            this.panel1.Controls.Add(this.lblPageTitle);
            this.panel1.Controls.Add(this.lblNotice);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(929, 34);
            this.panel1.TabIndex = 5;
            // 
            // chkShiftClickMode
            // 
            this.chkShiftClickMode.AutoSize = true;
            this.chkShiftClickMode.Location = new System.Drawing.Point(219, 7);
            this.chkShiftClickMode.Margin = new System.Windows.Forms.Padding(4);
            this.chkShiftClickMode.Name = "chkShiftClickMode";
            this.chkShiftClickMode.Size = new System.Drawing.Size(190, 21);
            this.chkShiftClickMode.TabIndex = 9;
            this.chkShiftClickMode.Text = "Use Shift+Click, Ctrl+Click";
            this.chkShiftClickMode.UseVisualStyleBackColor = true;
            this.chkShiftClickMode.CheckedChanged += new System.EventHandler(this.chkShiftClickSelect_CheckedChanged);
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageTitle.Location = new System.Drawing.Point(4, 6);
            this.lblPageTitle.Margin = new System.Windows.Forms.Padding(4, 6, 4, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(40, 17);
            this.lblPageTitle.TabIndex = 5;
            this.lblPageTitle.Text = "Title";
            // 
            // lblNotice
            // 
            this.lblNotice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotice.Location = new System.Drawing.Point(508, 6);
            this.lblNotice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNotice.Name = "lblNotice";
            this.lblNotice.Size = new System.Drawing.Size(417, 22);
            this.lblNotice.TabIndex = 6;
            this.lblNotice.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // gvQueryResults
            // 
            this.gvQueryResults.AllowDelete = true;
            this.gvQueryResults.AllowUserToAddRows = false;
            this.gvQueryResults.AllowUserToDeleteRows = false;
            this.gvQueryResults.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvQueryResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvQueryResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvQueryResults.DefaultCellStyle = dataGridViewCellStyle2;
            this.gvQueryResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvQueryResults.GridColor = System.Drawing.SystemColors.ControlLight;
            this.gvQueryResults.Location = new System.Drawing.Point(0, 34);
            this.gvQueryResults.Margin = new System.Windows.Forms.Padding(4);
            this.gvQueryResults.Name = "gvQueryResults";
            this.gvQueryResults.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvQueryResults.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gvQueryResults.ShiftClickSelect = false;
            this.gvQueryResults.Size = new System.Drawing.Size(929, 575);
            this.gvQueryResults.TabIndex = 6;
            this.gvQueryResults.SelectionChanged += new System.EventHandler(this.gvQueryResults_SelectionChanged);
            // 
            // GridViewDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gvQueryResults);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GridViewDisplayControl";
            this.Size = new System.Drawing.Size(929, 609);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvQueryResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Label lblNotice;
        private MyDataGridView gvQueryResults;
        private System.Windows.Forms.CheckBox chkShiftClickMode;
    }
}
