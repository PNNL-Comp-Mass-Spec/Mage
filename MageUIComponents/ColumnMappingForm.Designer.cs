namespace MageUIComponents
{
    partial class ColumnMappingForm
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
            this.panel11 = new System.Windows.Forms.Panel();
            this.DeleteColumnMappingBtn = new System.Windows.Forms.Button();
            this.panel10 = new System.Windows.Forms.Panel();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SaveColumnMappingsBtn = new System.Windows.Forms.Button();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.InsertColumnBtn = new System.Windows.Forms.Button();
            this.NewColumnBtn = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.DeleteColumnSpecBtn = new System.Windows.Forms.Button();
            this.ClearEditingPanelBtn = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.ReplaceExistingColumnMapptingBtn = new System.Windows.Forms.Button();
            this.AddNewColumnMappingBtn = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.MoveColSpecItemDownBtn = new System.Windows.Forms.Button();
            this.MoveColSpecItemUpBtn = new System.Windows.Forms.Button();
            this.SelectionPanel = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.EditingArea = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.LoadColumnListFromOutputBtn = new System.Windows.Forms.Button();
            this.LoadColumnListFromInputBtn = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.MappingDescriptionCtl = new System.Windows.Forms.TextBox();
            this.MappingNameCtl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.EditingPanel = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.RowPreviewPanel = new System.Windows.Forms.Panel();
            this.ColumnMappingDisplayList = new MageDisplayLib.GridViewDisplayControl();
            this.ColumnSpecEditingDisplayList = new MageDisplayLib.GridViewDisplayControl();
            this.RowPreviewDisplayList = new MageDisplayLib.GridViewDisplayControl();
            this.panel1.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SelectionPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.EditingArea.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel12.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.EditingPanel.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.RowPreviewPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.Controls.Add(this.panel11);
            this.panel1.Controls.Add(this.panel10);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(741, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(95, 224);
            this.panel1.TabIndex = 2;
            //
            // panel11
            //
            this.panel11.Controls.Add(this.DeleteColumnMappingBtn);
            this.panel11.Location = new System.Drawing.Point(5, 150);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(85, 65);
            this.panel11.TabIndex = 2;
            //
            // DeleteColumnMappingBtn
            //
            this.DeleteColumnMappingBtn.Location = new System.Drawing.Point(5, 7);
            this.DeleteColumnMappingBtn.Name = "DeleteColumnMappingBtn";
            this.DeleteColumnMappingBtn.Size = new System.Drawing.Size(75, 23);
            this.DeleteColumnMappingBtn.TabIndex = 0;
            this.DeleteColumnMappingBtn.Text = "Delete";
            this.DeleteColumnMappingBtn.UseVisualStyleBackColor = true;
            this.DeleteColumnMappingBtn.Click += new System.EventHandler(this.DeleteColumnMappingBtn_Click);
            //
            // panel10
            //
            this.panel10.Controls.Add(this.CancelBtn);
            this.panel10.Controls.Add(this.SaveColumnMappingsBtn);
            this.panel10.Controls.Add(this.CloseBtn);
            this.panel10.Location = new System.Drawing.Point(5, 5);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(85, 88);
            this.panel10.TabIndex = 1;
            //
            // CancelBtn
            //
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(5, 62);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            //
            // SaveColumnMappingsBtn
            //
            this.SaveColumnMappingsBtn.Enabled = false;
            this.SaveColumnMappingsBtn.Location = new System.Drawing.Point(5, 7);
            this.SaveColumnMappingsBtn.Name = "SaveColumnMappingsBtn";
            this.SaveColumnMappingsBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveColumnMappingsBtn.TabIndex = 0;
            this.SaveColumnMappingsBtn.Text = "Save";
            this.SaveColumnMappingsBtn.UseVisualStyleBackColor = true;
            this.SaveColumnMappingsBtn.Click += new System.EventHandler(this.SaveColumnMappingsBtn_Click);
            //
            // CloseBtn
            //
            this.CloseBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CloseBtn.Location = new System.Drawing.Point(5, 35);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(75, 23);
            this.CloseBtn.TabIndex = 0;
            this.CloseBtn.Text = "Select";
            this.CloseBtn.UseVisualStyleBackColor = true;
            //
            // panel2
            //
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel8);
            this.panel2.Controls.Add(this.panel7);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(736, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(95, 348);
            this.panel2.TabIndex = 6;
            //
            // panel3
            //
            this.panel3.Controls.Add(this.InsertColumnBtn);
            this.panel3.Controls.Add(this.NewColumnBtn);
            this.panel3.Location = new System.Drawing.Point(5, 279);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(85, 65);
            this.panel3.TabIndex = 5;
            //
            // InsertColumnBtn
            //
            this.InsertColumnBtn.Location = new System.Drawing.Point(5, 7);
            this.InsertColumnBtn.Name = "InsertColumnBtn";
            this.InsertColumnBtn.Size = new System.Drawing.Size(75, 23);
            this.InsertColumnBtn.TabIndex = 5;
            this.InsertColumnBtn.Text = "Insert";
            this.InsertColumnBtn.UseVisualStyleBackColor = true;
            this.InsertColumnBtn.Click += new System.EventHandler(this.InsertColumnBtn_Click);
            //
            // NewColumnBtn
            //
            this.NewColumnBtn.Location = new System.Drawing.Point(5, 35);
            this.NewColumnBtn.Name = "NewColumnBtn";
            this.NewColumnBtn.Size = new System.Drawing.Size(75, 23);
            this.NewColumnBtn.TabIndex = 4;
            this.NewColumnBtn.Text = "Append";
            this.NewColumnBtn.UseVisualStyleBackColor = true;
            this.NewColumnBtn.Click += new System.EventHandler(this.NewColumnBtn_Click);
            //
            // panel8
            //
            this.panel8.Controls.Add(this.DeleteColumnSpecBtn);
            this.panel8.Controls.Add(this.ClearEditingPanelBtn);
            this.panel8.Location = new System.Drawing.Point(5, 195);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(85, 65);
            this.panel8.TabIndex = 3;
            //
            // DeleteColumnSpecBtn
            //
            this.DeleteColumnSpecBtn.Location = new System.Drawing.Point(5, 7);
            this.DeleteColumnSpecBtn.Name = "DeleteColumnSpecBtn";
            this.DeleteColumnSpecBtn.Size = new System.Drawing.Size(75, 23);
            this.DeleteColumnSpecBtn.TabIndex = 0;
            this.DeleteColumnSpecBtn.Text = "Delete";
            this.DeleteColumnSpecBtn.UseVisualStyleBackColor = true;
            this.DeleteColumnSpecBtn.Click += new System.EventHandler(this.DeleteColumnSpecBtn_Click);
            //
            // ClearEditingPanelBtn
            //
            this.ClearEditingPanelBtn.Location = new System.Drawing.Point(5, 35);
            this.ClearEditingPanelBtn.Name = "ClearEditingPanelBtn";
            this.ClearEditingPanelBtn.Size = new System.Drawing.Size(75, 23);
            this.ClearEditingPanelBtn.TabIndex = 0;
            this.ClearEditingPanelBtn.Text = "Clear";
            this.ClearEditingPanelBtn.UseVisualStyleBackColor = true;
            this.ClearEditingPanelBtn.Click += new System.EventHandler(this.ClearEditingPanelBtn_Click);
            //
            // panel7
            //
            this.panel7.Controls.Add(this.ReplaceExistingColumnMapptingBtn);
            this.panel7.Controls.Add(this.AddNewColumnMappingBtn);
            this.panel7.Location = new System.Drawing.Point(5, 6);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(85, 65);
            this.panel7.TabIndex = 2;
            //
            // ReplaceExistingColumnMapptingBtn
            //
            this.ReplaceExistingColumnMapptingBtn.Location = new System.Drawing.Point(5, 35);
            this.ReplaceExistingColumnMapptingBtn.Name = "ReplaceExistingColumnMapptingBtn";
            this.ReplaceExistingColumnMapptingBtn.Size = new System.Drawing.Size(75, 23);
            this.ReplaceExistingColumnMapptingBtn.TabIndex = 0;
            this.ReplaceExistingColumnMapptingBtn.Text = "Replace";
            this.ReplaceExistingColumnMapptingBtn.UseVisualStyleBackColor = true;
            this.ReplaceExistingColumnMapptingBtn.Click += new System.EventHandler(this.ReplaceExistingColumnMappingBtn_Click);
            //
            // AddNewColumnMappingBtn
            //
            this.AddNewColumnMappingBtn.Location = new System.Drawing.Point(5, 7);
            this.AddNewColumnMappingBtn.Name = "AddNewColumnMappingBtn";
            this.AddNewColumnMappingBtn.Size = new System.Drawing.Size(75, 23);
            this.AddNewColumnMappingBtn.TabIndex = 0;
            this.AddNewColumnMappingBtn.Text = "Add";
            this.AddNewColumnMappingBtn.UseVisualStyleBackColor = true;
            this.AddNewColumnMappingBtn.Click += new System.EventHandler(this.AddNewColumnMappingBtn_Click);
            //
            // panel5
            //
            this.panel5.Controls.Add(this.MoveColSpecItemDownBtn);
            this.panel5.Controls.Add(this.MoveColSpecItemUpBtn);
            this.panel5.Location = new System.Drawing.Point(5, 112);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(85, 65);
            this.panel5.TabIndex = 1;
            //
            // MoveColSpecItemDownBtn
            //
            this.MoveColSpecItemDownBtn.Location = new System.Drawing.Point(5, 35);
            this.MoveColSpecItemDownBtn.Name = "MoveColSpecItemDownBtn";
            this.MoveColSpecItemDownBtn.Size = new System.Drawing.Size(75, 23);
            this.MoveColSpecItemDownBtn.TabIndex = 0;
            this.MoveColSpecItemDownBtn.Text = "Down";
            this.MoveColSpecItemDownBtn.UseVisualStyleBackColor = true;
            this.MoveColSpecItemDownBtn.Click += new System.EventHandler(this.MoveColSpecItemDownBtn_Click);
            //
            // MoveColSpecItemUpBtn
            //
            this.MoveColSpecItemUpBtn.Location = new System.Drawing.Point(5, 7);
            this.MoveColSpecItemUpBtn.Name = "MoveColSpecItemUpBtn";
            this.MoveColSpecItemUpBtn.Size = new System.Drawing.Size(75, 23);
            this.MoveColSpecItemUpBtn.TabIndex = 0;
            this.MoveColSpecItemUpBtn.Text = "Up";
            this.MoveColSpecItemUpBtn.UseVisualStyleBackColor = true;
            this.MoveColSpecItemUpBtn.Click += new System.EventHandler(this.MoveColSpecItemUpBtn_Click);
            //
            // SelectionPanel
            //
            this.SelectionPanel.BackColor = System.Drawing.SystemColors.Control;
            this.SelectionPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectionPanel.Controls.Add(this.panel4);
            this.SelectionPanel.Controls.Add(this.panel1);
            this.SelectionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectionPanel.Location = new System.Drawing.Point(0, 0);
            this.SelectionPanel.Name = "SelectionPanel";
            this.SelectionPanel.Size = new System.Drawing.Size(838, 226);
            this.SelectionPanel.TabIndex = 7;
            //
            // panel4
            //
            this.panel4.Controls.Add(this.ColumnMappingDisplayList);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5);
            this.panel4.Size = new System.Drawing.Size(741, 224);
            this.panel4.TabIndex = 3;
            //
            // EditingArea
            //
            this.EditingArea.BackColor = System.Drawing.SystemColors.Control;
            this.EditingArea.Controls.Add(this.panel6);
            this.EditingArea.Controls.Add(this.panel9);
            this.EditingArea.Controls.Add(this.panel2);
            this.EditingArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditingArea.Location = new System.Drawing.Point(0, 0);
            this.EditingArea.Name = "EditingArea";
            this.EditingArea.Padding = new System.Windows.Forms.Padding(5);
            this.EditingArea.Size = new System.Drawing.Size(836, 358);
            this.EditingArea.TabIndex = 8;
            //
            // panel6
            //
            this.panel6.Controls.Add(this.ColumnSpecEditingDisplayList);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(5, 109);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(5);
            this.panel6.Size = new System.Drawing.Size(731, 244);
            this.panel6.TabIndex = 7;
            //
            // panel9
            //
            this.panel9.Controls.Add(this.panel12);
            this.panel9.Controls.Add(this.MappingDescriptionCtl);
            this.panel9.Controls.Add(this.MappingNameCtl);
            this.panel9.Controls.Add(this.label2);
            this.panel9.Controls.Add(this.label1);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(5, 5);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(731, 104);
            this.panel9.TabIndex = 10;
            //
            // panel12
            //
            this.panel12.BackColor = System.Drawing.SystemColors.Control;
            this.panel12.Controls.Add(this.LoadColumnListFromOutputBtn);
            this.panel12.Controls.Add(this.LoadColumnListFromInputBtn);
            this.panel12.Controls.Add(this.label7);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(0, 0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(731, 30);
            this.panel12.TabIndex = 10;
            //
            // LoadColumnListFromOutputBtn
            //
            this.LoadColumnListFromOutputBtn.Location = new System.Drawing.Point(396, 4);
            this.LoadColumnListFromOutputBtn.Name = "LoadColumnListFromOutputBtn";
            this.LoadColumnListFromOutputBtn.Size = new System.Drawing.Size(175, 23);
            this.LoadColumnListFromOutputBtn.TabIndex = 2;
            this.LoadColumnListFromOutputBtn.Text = "Load Column List From Output";
            this.LoadColumnListFromOutputBtn.UseVisualStyleBackColor = true;
            this.LoadColumnListFromOutputBtn.Click += new System.EventHandler(this.LoadColumnListFromOutputBtn_Click);
            //
            // LoadColumnListFromInputBtn
            //
            this.LoadColumnListFromInputBtn.Location = new System.Drawing.Point(217, 4);
            this.LoadColumnListFromInputBtn.Name = "LoadColumnListFromInputBtn";
            this.LoadColumnListFromInputBtn.Size = new System.Drawing.Size(175, 23);
            this.LoadColumnListFromInputBtn.TabIndex = 1;
            this.LoadColumnListFromInputBtn.Text = "Load Column List From Input";
            this.LoadColumnListFromInputBtn.UseVisualStyleBackColor = true;
            this.LoadColumnListFromInputBtn.Click += new System.EventHandler(this.LoadColumnListFromInputBtn_Click);
            //
            // label7
            //
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Edit Column Mapping";
            //
            // MappingDescriptionCtl
            //
            this.MappingDescriptionCtl.AcceptsTab = true;
            this.MappingDescriptionCtl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MappingDescriptionCtl.Location = new System.Drawing.Point(80, 72);
            this.MappingDescriptionCtl.Multiline = true;
            this.MappingDescriptionCtl.Name = "MappingDescriptionCtl";
            this.MappingDescriptionCtl.Size = new System.Drawing.Size(637, 29);
            this.MappingDescriptionCtl.TabIndex = 3;
            //
            // MappingNameCtl
            //
            this.MappingNameCtl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MappingNameCtl.Location = new System.Drawing.Point(80, 38);
            this.MappingNameCtl.Name = "MappingNameCtl";
            this.MappingNameCtl.Size = new System.Drawing.Size(637, 20);
            this.MappingNameCtl.TabIndex = 2;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Description";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            //
            // splitContainer1
            //
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.SelectionPanel);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.Controls.Add(this.EditingPanel);
            this.splitContainer1.Size = new System.Drawing.Size(838, 592);
            this.splitContainer1.SplitterDistance = 226;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 10;
            //
            // EditingPanel
            //
            this.EditingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EditingPanel.Controls.Add(this.EditingArea);
            this.EditingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditingPanel.Location = new System.Drawing.Point(0, 0);
            this.EditingPanel.Name = "EditingPanel";
            this.EditingPanel.Size = new System.Drawing.Size(838, 360);
            this.EditingPanel.TabIndex = 10;
            //
            // splitContainer2
            //
            this.splitContainer2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitContainer2.Panel1
            //
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            //
            // splitContainer2.Panel2
            //
            this.splitContainer2.Panel2.Controls.Add(this.RowPreviewPanel);
            this.splitContainer2.Size = new System.Drawing.Size(838, 770);
            this.splitContainer2.SplitterDistance = 592;
            this.splitContainer2.SplitterWidth = 6;
            this.splitContainer2.TabIndex = 11;
            //
            // RowPreviewPanel
            //
            this.RowPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RowPreviewPanel.Controls.Add(this.RowPreviewDisplayList);
            this.RowPreviewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RowPreviewPanel.Location = new System.Drawing.Point(0, 0);
            this.RowPreviewPanel.Name = "RowPreviewPanel";
            this.RowPreviewPanel.Padding = new System.Windows.Forms.Padding(5);
            this.RowPreviewPanel.Size = new System.Drawing.Size(838, 172);
            this.RowPreviewPanel.TabIndex = 0;
            //
            // ColumnMappingDisplayList
            //
            this.ColumnMappingDisplayList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColumnMappingDisplayList.HeaderVisible = true;
            this.ColumnMappingDisplayList.ItemBlockSize = 100;
            this.ColumnMappingDisplayList.Location = new System.Drawing.Point(5, 5);
            this.ColumnMappingDisplayList.Name = "ColumnMappingDisplayList";
            this.ColumnMappingDisplayList.Notice = "";
            this.ColumnMappingDisplayList.PageTitle = "Title";
            this.ColumnMappingDisplayList.Size = new System.Drawing.Size(731, 214);
            this.ColumnMappingDisplayList.TabIndex = 0;
            //
            // ColumnSpecEditingDisplayList
            //
            this.ColumnSpecEditingDisplayList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColumnSpecEditingDisplayList.HeaderVisible = true;
            this.ColumnSpecEditingDisplayList.ItemBlockSize = 100;
            this.ColumnSpecEditingDisplayList.Location = new System.Drawing.Point(5, 5);
            this.ColumnSpecEditingDisplayList.Name = "ColumnSpecEditingDisplayList";
            this.ColumnSpecEditingDisplayList.Notice = "";
            this.ColumnSpecEditingDisplayList.PageTitle = "Title";
            this.ColumnSpecEditingDisplayList.Size = new System.Drawing.Size(721, 234);
            this.ColumnSpecEditingDisplayList.TabIndex = 0;
            //
            // RowPreviewDisplayList
            //
            this.RowPreviewDisplayList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RowPreviewDisplayList.HeaderVisible = true;
            this.RowPreviewDisplayList.ItemBlockSize = 100;
            this.RowPreviewDisplayList.Location = new System.Drawing.Point(5, 5);
            this.RowPreviewDisplayList.Name = "RowPreviewDisplayList";
            this.RowPreviewDisplayList.Notice = "";
            this.RowPreviewDisplayList.PageTitle = "Title";
            this.RowPreviewDisplayList.Size = new System.Drawing.Size(826, 160);
            this.RowPreviewDisplayList.TabIndex = 0;
            //
            // ColumnMappingForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 770);
            this.Controls.Add(this.splitContainer2);
            this.MinimumSize = new System.Drawing.Size(700, 660);
            this.Name = "ColumnMappingForm";
            this.ShowInTaskbar = false;
            this.Text = "Column Mapping Editor";
            this.Load += new System.EventHandler(this.ColumnMappingForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColumnMappingForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.SelectionPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.EditingArea.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.EditingPanel.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.RowPreviewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button ClearEditingPanelBtn;
        private System.Windows.Forms.Button MoveColSpecItemDownBtn;
        private System.Windows.Forms.Button MoveColSpecItemUpBtn;
        private System.Windows.Forms.Panel SelectionPanel;
        private System.Windows.Forms.Panel EditingArea;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.TextBox MappingDescriptionCtl;
        private System.Windows.Forms.TextBox MappingNameCtl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button SaveColumnMappingsBtn;
        private System.Windows.Forms.Button DeleteColumnMappingBtn;
        private System.Windows.Forms.Button DeleteColumnSpecBtn;
        private System.Windows.Forms.Button ReplaceExistingColumnMapptingBtn;
        private System.Windows.Forms.Button AddNewColumnMappingBtn;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel EditingPanel;
        private System.Windows.Forms.Panel RowPreviewPanel;
        private System.Windows.Forms.Button NewColumnBtn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button LoadColumnListFromOutputBtn;
        private System.Windows.Forms.Button LoadColumnListFromInputBtn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button InsertColumnBtn;
        private MageDisplayLib.GridViewDisplayControl ColumnMappingDisplayList;
        private MageDisplayLib.GridViewDisplayControl ColumnSpecEditingDisplayList;
        private MageDisplayLib.GridViewDisplayControl RowPreviewDisplayList;
    }
}