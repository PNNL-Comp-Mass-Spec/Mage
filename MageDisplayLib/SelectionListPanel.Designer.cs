namespace MageDisplayLib {
    partial class SelectionListPanel {
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
            this.EntityLabelCtl = new System.Windows.Forms.Label();
            this.ItemIDCtl = new System.Windows.Forms.TextBox();
            this.listDisplayControl1 = new MageDisplayLib.ListDisplayControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.listDisplayControl1);
            this.panel1.Controls.Add(this.EntityLabelCtl);
            this.panel1.Controls.Add(this.ItemIDCtl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(610, 510);
            this.panel1.TabIndex = 0;
            // 
            // EntityLabelCtl
            // 
            this.EntityLabelCtl.AutoSize = true;
            this.EntityLabelCtl.Location = new System.Drawing.Point(9, 11);
            this.EntityLabelCtl.Name = "EntityLabelCtl";
            this.EntityLabelCtl.Size = new System.Drawing.Size(47, 13);
            this.EntityLabelCtl.TabIndex = 4;
            this.EntityLabelCtl.Text = "Entity ID";
            // 
            // ItemIDCtl
            // 
            this.ItemIDCtl.Location = new System.Drawing.Point(106, 8);
            this.ItemIDCtl.Name = "ItemIDCtl";
            this.ItemIDCtl.Size = new System.Drawing.Size(126, 20);
            this.ItemIDCtl.TabIndex = 3;
            this.ItemIDCtl.Text = "100";
            // 
            // listDisplayControl1
            // 
            this.listDisplayControl1.Accumulator = null;
            this.listDisplayControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listDisplayControl1.HeaderVisible = true;
            this.listDisplayControl1.Location = new System.Drawing.Point(5, 40);
            this.listDisplayControl1.Name = "listDisplayControl1";
            this.listDisplayControl1.Notice = "";
            this.listDisplayControl1.PageTitle = "Title";
            this.listDisplayControl1.Size = new System.Drawing.Size(600, 460);
            this.listDisplayControl1.TabIndex = 6;
            this.listDisplayControl1.SelectionChanged += new System.EventHandler<System.EventArgs>(this.listDisplayControl1_SelectionChanged);
            // 
            // SelectionListPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "SelectionListPanel";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(620, 520);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label EntityLabelCtl;
        private System.Windows.Forms.TextBox ItemIDCtl;
        private MageDisplayLib.ListDisplayControl listDisplayControl1;
    }
}
