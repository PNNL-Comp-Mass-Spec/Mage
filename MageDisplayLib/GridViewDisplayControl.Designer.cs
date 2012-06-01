namespace MageDisplayLib {
    partial class GridViewDisplayControl {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(697, 28);
            this.panel1.TabIndex = 5;
            // 
            // chkShiftClickMode
            // 
            this.chkShiftClickMode.AutoSize = true;
            this.chkShiftClickMode.Location = new System.Drawing.Point(164, 6);
            this.chkShiftClickMode.Name = "chkShiftClickMode";
            this.chkShiftClickMode.Size = new System.Drawing.Size(148, 17);
            this.chkShiftClickMode.TabIndex = 9;
            this.chkShiftClickMode.Text = "Use Shift+Click, Ctrl+Click";
            this.chkShiftClickMode.UseVisualStyleBackColor = true;
            this.chkShiftClickMode.CheckedChanged += new System.EventHandler(this.chkShiftClickSelect_CheckedChanged);
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPageTitle.Location = new System.Drawing.Point(3, 5);
            this.lblPageTitle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(32, 13);
            this.lblPageTitle.TabIndex = 5;
            this.lblPageTitle.Text = "Title";
            // 
            // lblNotice
            // 
            this.lblNotice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNotice.Location = new System.Drawing.Point(381, 5);
            this.lblNotice.Name = "lblNotice";
            this.lblNotice.Size = new System.Drawing.Size(313, 18);
            this.lblNotice.TabIndex = 6;
            this.lblNotice.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // gvQueryResults
            // 
            this.gvQueryResults.AllowUserToAddRows = false;
            this.gvQueryResults.AllowUserToDeleteRows = false;
            this.gvQueryResults.AllowUserToResizeColumns = true;
            this.gvQueryResults.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvQueryResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gvQueryResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvQueryResults.DefaultCellStyle = dataGridViewCellStyle5;
            this.gvQueryResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvQueryResults.GridColor = System.Drawing.SystemColors.ControlLight;
            this.gvQueryResults.Location = new System.Drawing.Point(0, 28);
            this.gvQueryResults.Name = "gvQueryResults";
            this.gvQueryResults.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvQueryResults.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.gvQueryResults.ShiftClickSelect = false;
            this.gvQueryResults.Size = new System.Drawing.Size(697, 467);
            this.gvQueryResults.TabIndex = 6;
            this.gvQueryResults.SelectionChanged += new System.EventHandler(this.gvQueryResults_SelectionChanged);
            // 
            // GridViewDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gvQueryResults);
            this.Controls.Add(this.panel1);
            this.Name = "GridViewDisplayControl";
            this.Size = new System.Drawing.Size(697, 495);
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
