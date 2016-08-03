namespace MageUIComponents
{
    partial class IncrementalParameterSubPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.ParamIncrementCtl = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ParamNameCtl = new System.Windows.Forms.TextBox();
            this.ActiveCtl = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ParamLowerCtl = new System.Windows.Forms.TextBox();
            this.ParamUpperCtl = new System.Windows.Forms.TextBox();
            this.OperationCtl = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 11;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ParamIncrementCtl, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.ParamNameCtl, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.ActiveCtl, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.ParamLowerCtl, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.ParamUpperCtl, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.OperationCtl, 10, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 9, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(820, 30);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // ParamIncrementCtl
            // 
            this.ParamIncrementCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ParamIncrementCtl.Location = new System.Drawing.Point(553, 5);
            this.ParamIncrementCtl.Name = "ParamIncrementCtl";
            this.ParamIncrementCtl.Size = new System.Drawing.Size(94, 20);
            this.ParamIncrementCtl.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(493, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Increment";
            // 
            // ParamNameCtl
            // 
            this.ParamNameCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ParamNameCtl.Location = new System.Drawing.Point(83, 5);
            this.ParamNameCtl.Name = "ParamNameCtl";
            this.ParamNameCtl.Size = new System.Drawing.Size(94, 20);
            this.ParamNameCtl.TabIndex = 0;
            // 
            // ActiveCtl
            // 
            this.ActiveCtl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ActiveCtl.AutoSize = true;
            this.ActiveCtl.Checked = true;
            this.ActiveCtl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ActiveCtl.Location = new System.Drawing.Point(3, 8);
            this.ActiveCtl.Name = "ActiveCtl";
            this.ActiveCtl.Size = new System.Drawing.Size(15, 14);
            this.ActiveCtl.TabIndex = 2;
            this.ActiveCtl.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(191, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Lower";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(341, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Upper";
            // 
            // ParamLowerCtl
            // 
            this.ParamLowerCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ParamLowerCtl.Location = new System.Drawing.Point(233, 5);
            this.ParamLowerCtl.Name = "ParamLowerCtl";
            this.ParamLowerCtl.Size = new System.Drawing.Size(94, 20);
            this.ParamLowerCtl.TabIndex = 0;
            // 
            // ParamUpperCtl
            // 
            this.ParamUpperCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ParamUpperCtl.Location = new System.Drawing.Point(383, 5);
            this.ParamUpperCtl.Name = "ParamUpperCtl";
            this.ParamUpperCtl.Size = new System.Drawing.Size(94, 20);
            this.ParamUpperCtl.TabIndex = 0;
            // 
            // OperationCtl
            // 
            this.OperationCtl.FormattingEnabled = true;
            this.OperationCtl.Items.AddRange(new object[] {
            "=",
            ">",
            "<",
            ">=",
            "<=",
            ">;<",
            ">=;<",
            ">;<=",
            ">=;<="});
            this.OperationCtl.Location = new System.Drawing.Point(723, 3);
            this.OperationCtl.Name = "OperationCtl";
            this.OperationCtl.Size = new System.Drawing.Size(94, 21);
            this.OperationCtl.TabIndex = 3;
            this.OperationCtl.Text = "=";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(664, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Operation";
            // 
            // IncrementalParameterSubPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "IncrementalParameterSubPanel";
            this.Size = new System.Drawing.Size(820, 30);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ParamIncrementCtl;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ParamNameCtl;
        private System.Windows.Forms.CheckBox ActiveCtl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ParamLowerCtl;
        private System.Windows.Forms.TextBox ParamUpperCtl;
        private System.Windows.Forms.ComboBox OperationCtl;
        private System.Windows.Forms.Label label5;
    }
}
