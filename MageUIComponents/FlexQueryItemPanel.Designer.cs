namespace MageUIComponents
{
    partial class FlexQueryItemPanel
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.RelationCtl = new System.Windows.Forms.ComboBox();
            this.ColumnCtl = new System.Windows.Forms.ComboBox();
            this.ComparisonCtl = new System.Windows.Forms.ComboBox();
            this.ValueCtl = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.RelationCtl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ColumnCtl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ComparisonCtl, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.ValueCtl, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 26);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // RelationCtl
            // 
            this.RelationCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RelationCtl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RelationCtl.FormattingEnabled = true;
            this.RelationCtl.Location = new System.Drawing.Point(3, 3);
            this.RelationCtl.Name = "RelationCtl";
            this.RelationCtl.Size = new System.Drawing.Size(54, 21);
            this.RelationCtl.TabIndex = 0;
            this.RelationCtl.SelectedIndexChanged += new System.EventHandler(this.RelationCtl_SelectedIndexChanged);
            // 
            // ColumnCtl
            // 
            this.ColumnCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ColumnCtl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColumnCtl.FormattingEnabled = true;
            this.ColumnCtl.Location = new System.Drawing.Point(62, 2);
            this.ColumnCtl.Margin = new System.Windows.Forms.Padding(2);
            this.ColumnCtl.Name = "ColumnCtl";
            this.ColumnCtl.Size = new System.Drawing.Size(142, 21);
            this.ColumnCtl.TabIndex = 1;
            this.ColumnCtl.SelectedIndexChanged += new System.EventHandler(this.ColumnCtl_SelectedIndexChanged);
            // 
            // ComparisonCtl
            // 
            this.ComparisonCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ComparisonCtl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComparisonCtl.FormattingEnabled = true;
            this.ComparisonCtl.Location = new System.Drawing.Point(209, 3);
            this.ComparisonCtl.Name = "ComparisonCtl";
            this.ComparisonCtl.Size = new System.Drawing.Size(140, 21);
            this.ComparisonCtl.TabIndex = 2;
            // 
            // ValueCtl
            // 
            this.ValueCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueCtl.Location = new System.Drawing.Point(355, 3);
            this.ValueCtl.Name = "ValueCtl";
            this.ValueCtl.Size = new System.Drawing.Size(142, 20);
            this.ValueCtl.TabIndex = 3;
            // 
            // FlexQueryItemPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FlexQueryItemPanel";
            this.Size = new System.Drawing.Size(500, 26);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox RelationCtl;
        private System.Windows.Forms.ComboBox ColumnCtl;
        private System.Windows.Forms.ComboBox ComparisonCtl;
        private System.Windows.Forms.TextBox ValueCtl;
    }
}
