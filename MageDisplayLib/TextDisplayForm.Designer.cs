namespace MageDisplayLib
{

    /// <summary>
    /// Displays a textbox
    /// </summary>
    partial class TextDisplayForm
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
            this.textDisplayControl1 = new MageDisplayLib.TextDisplayControl();
            this.CloseCtl = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textDisplayControl1
            // 
            this.textDisplayControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textDisplayControl1.Contents = "";
            this.textDisplayControl1.Location = new System.Drawing.Point(12, 12);
            this.textDisplayControl1.Name = "textDisplayControl1";
            this.textDisplayControl1.Padding = new System.Windows.Forms.Padding(5);
            this.textDisplayControl1.ReadOnly = false;
            this.textDisplayControl1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textDisplayControl1.Size = new System.Drawing.Size(507, 268);
            this.textDisplayControl1.TabIndex = 0;
            // 
            // CloseCtl
            // 
            this.CloseCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CloseCtl.Location = new System.Drawing.Point(16, 282);
            this.CloseCtl.Name = "CloseCtl";
            this.CloseCtl.Size = new System.Drawing.Size(75, 23);
            this.CloseCtl.TabIndex = 1;
            this.CloseCtl.Text = "&Close";
            this.CloseCtl.UseVisualStyleBackColor = true;
            this.CloseCtl.Click += new System.EventHandler(this.CloseCtl_Click);
            // 
            // TextDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 314);
            this.Controls.Add(this.CloseCtl);
            this.Controls.Add(this.textDisplayControl1);
            this.Name = "TextDisplayForm";
            this.Text = "TextDisplayForm";
            this.ResumeLayout(false);

        }

        #endregion

        private TextDisplayControl textDisplayControl1;
        private System.Windows.Forms.Button CloseCtl;
    }
}