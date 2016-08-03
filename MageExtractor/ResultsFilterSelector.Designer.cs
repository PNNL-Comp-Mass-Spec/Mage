namespace MageExtractor
{
    partial class ResultsFilterSelector
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
            this.label1 = new System.Windows.Forms.Label();
            this.FilterSetIDCtl = new System.Windows.Forms.TextBox();
            this.gridViewDisplayControl1 = new MageDisplayLib.GridViewDisplayControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.FilterSetIDCtl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(474, 35);
            this.panel1.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Filter Set ID";
            // 
            // FilterSetIDCtl
            // 
            this.FilterSetIDCtl.Location = new System.Drawing.Point(72, 7);
            this.FilterSetIDCtl.Name = "FilterSetIDCtl";
            this.FilterSetIDCtl.Size = new System.Drawing.Size(126, 20);
            this.FilterSetIDCtl.TabIndex = 0;
            this.FilterSetIDCtl.Text = "100";
            // 
            // gridViewDisplayControl1
            // 
            this.gridViewDisplayControl1.AllowDisableShiftClickMode = true;
            this.gridViewDisplayControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridViewDisplayControl1.HeaderVisible = true;
            this.gridViewDisplayControl1.ItemBlockSize = 100;
            this.gridViewDisplayControl1.Location = new System.Drawing.Point(0, 35);
            this.gridViewDisplayControl1.MultiSelect = true;
            this.gridViewDisplayControl1.Name = "gridViewDisplayControl1";
            this.gridViewDisplayControl1.Notice = "";
            this.gridViewDisplayControl1.PageTitle = "Title";
            this.gridViewDisplayControl1.Size = new System.Drawing.Size(474, 376);
            this.gridViewDisplayControl1.TabIndex = 8;
            this.gridViewDisplayControl1.SelectionChanged += new System.EventHandler<System.EventArgs>(this.List_SelectionChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button2);
            this.panel3.Controls.Add(this.button1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 411);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(474, 37);
            this.panel3.TabIndex = 10;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(387, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(306, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ResultsFilterSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 448);
            this.Controls.Add(this.gridViewDisplayControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Name = "ResultsFilterSelector";
            this.Text = "Results Filter Selector";
            this.Load += new System.EventHandler(this.FilterPanel_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FilterSetIDCtl;
        private MageDisplayLib.GridViewDisplayControl gridViewDisplayControl1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}