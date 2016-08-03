namespace MageUIComponents
{
    partial class FilterSelectionForm
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
            this.CancelCtl = new System.Windows.Forms.Button();
            this.OkCtl = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.FilterNameCtl = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.gridViewDisplayControl1 = new MageDisplayLib.GridViewDisplayControl();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CancelCtl);
            this.panel1.Controls.Add(this.OkCtl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 375);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 37);
            this.panel1.TabIndex = 4;
            // 
            // CancelCtl
            // 
            this.CancelCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelCtl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelCtl.Location = new System.Drawing.Point(402, 6);
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
            this.OkCtl.Location = new System.Drawing.Point(321, 6);
            this.OkCtl.Name = "OkCtl";
            this.OkCtl.Size = new System.Drawing.Size(75, 23);
            this.OkCtl.TabIndex = 0;
            this.OkCtl.Text = "OK";
            this.OkCtl.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.FilterNameCtl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(489, 30);
            this.panel2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Filter:";
            // 
            // FilterNameCtl
            // 
            this.FilterNameCtl.AutoSize = true;
            this.FilterNameCtl.Location = new System.Drawing.Point(49, 10);
            this.FilterNameCtl.Name = "FilterNameCtl";
            this.FilterNameCtl.Size = new System.Drawing.Size(44, 13);
            this.FilterNameCtl.TabIndex = 0;
            this.FilterNameCtl.Text = "All Pass";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.gridViewDisplayControl1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 30);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(489, 345);
            this.panel3.TabIndex = 7;
            // 
            // gridViewDisplayControl1
            // 
            this.gridViewDisplayControl1.AllowDisableShiftClickMode = true;
            this.gridViewDisplayControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridViewDisplayControl1.HeaderVisible = true;
            this.gridViewDisplayControl1.ItemBlockSize = 100;
            this.gridViewDisplayControl1.Location = new System.Drawing.Point(5, 5);
            this.gridViewDisplayControl1.MultiSelect = true;
            this.gridViewDisplayControl1.Name = "gridViewDisplayControl1";
            this.gridViewDisplayControl1.Notice = "";
            this.gridViewDisplayControl1.PageTitle = "Title";
            this.gridViewDisplayControl1.Size = new System.Drawing.Size(479, 335);
            this.gridViewDisplayControl1.TabIndex = 0;
            this.gridViewDisplayControl1.SelectionChanged += new System.EventHandler<System.EventArgs>(this.DisplayControl_SelectionChanged);
            // 
            // FilterSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 412);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FilterSelectionForm";
            this.Text = "Filter Selection";
            this.Load += new System.EventHandler(this.FilterSelectionForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button CancelCtl;
        private System.Windows.Forms.Button OkCtl;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label FilterNameCtl;
        private System.Windows.Forms.Panel panel3;
        private MageDisplayLib.GridViewDisplayControl gridViewDisplayControl1;
    }
}