namespace MageDemo
{
    partial class MageDemo
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
            this.gridViewDisplayControl1 = new MageDisplayLib.GridViewDisplayControl();
            this.cmdGetData = new System.Windows.Forms.Button();
            this.pnlStatus = new MageDisplayLib.StatusPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            //
            // gridViewDisplayControl1
            //
            this.gridViewDisplayControl1.AllowDisableShiftClickMode = true;
            this.gridViewDisplayControl1.AutoSizeColumnWidths = false;
            this.gridViewDisplayControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridViewDisplayControl1.HeaderVisible = true;
            this.gridViewDisplayControl1.ItemBlockSize = 25;
            this.gridViewDisplayControl1.Location = new System.Drawing.Point(0, 60);
            this.gridViewDisplayControl1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.gridViewDisplayControl1.MultiSelect = true;
            this.gridViewDisplayControl1.Name = "gridViewDisplayControl1";
            this.gridViewDisplayControl1.Notice = "";
            this.gridViewDisplayControl1.PageTitle = "Title";
            this.gridViewDisplayControl1.Size = new System.Drawing.Size(863, 583);
            this.gridViewDisplayControl1.TabIndex = 0;
            //
            // cmdGetData
            //
            this.cmdGetData.Location = new System.Drawing.Point(16, 16);
            this.cmdGetData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmdGetData.Name = "cmdGetData";
            this.cmdGetData.Size = new System.Drawing.Size(100, 28);
            this.cmdGetData.TabIndex = 1;
            this.cmdGetData.Text = "Get Data";
            this.cmdGetData.UseVisualStyleBackColor = true;
            this.cmdGetData.Click += new System.EventHandler(this.cmdGetData_Click);
            //
            // pnlStatus
            //
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.EnableCancel = true;
            this.pnlStatus.Location = new System.Drawing.Point(0, 643);
            this.pnlStatus.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.OwnerControl = this;
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.pnlStatus.ShowCancel = true;
            this.pnlStatus.Size = new System.Drawing.Size(863, 52);
            this.pnlStatus.TabIndex = 2;
            //
            // panel1
            //
            this.panel1.Controls.Add(this.cmdGetData);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(863, 60);
            this.panel1.TabIndex = 3;
            //
            // MageDemo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 695);
            this.Controls.Add(this.gridViewDisplayControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlStatus);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MageDemo";
            this.Text = "Mage Campaign Data Demo";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MageDisplayLib.GridViewDisplayControl gridViewDisplayControl1;
        private System.Windows.Forms.Button cmdGetData;
        private MageDisplayLib.StatusPanel pnlStatus;
        private System.Windows.Forms.Panel panel1;
    }
}

