﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MageDisplayLib
{
    /// <summary>
    /// Implements a floating entry field editor that overlays display cells
    /// </summary>
    public class ListViewCellEditor
    {
        // Ignore Spelling: Mage, subitem

        /// <summary>
        /// Default cell editor
        /// </summary>
        private TextBox mDefaultCellEditor;

        /// <summary>
        /// The ListView control that we provide cell editing for
        /// </summary>
        private readonly ListViewEx mListView;

        /// <summary>
        /// The ListViewItem representing the row
        /// containing the cell currently being edited
        /// </summary>
        private ListViewItem mListViewItemUnderEdit;

        /// <summary>
        /// The column index of the cell currently being edited
        /// </summary>
        private int mSubItemUnderEdit;

        /// <summary>
        /// Position of last mouse click (used to determine which cell to edit)
        /// </summary>
        private int mLastClickX;

        // ReSharper disable once NotAccessedField.Local
        private int mLastClickY;

        /// <summary>
        /// Position of last horizontal scroll
        /// </summary>
        private int mLastScrollX;

        /// <summary>
        /// List of ComboBox cell editors
        /// </summary>
        private readonly List<Control> mPickers = new();

        /// <summary>
        /// Association between column index and its ComboBox cell editor
        /// </summary>
        private readonly Dictionary<int, int> mColumnPickers = new();

        /// <summary>
        /// Defines whether the cell editor can make changes to underlying cell
        /// </summary>
        public bool Editable
        {
            get => mDefaultCellEditor.ReadOnly;
            set => mDefaultCellEditor.ReadOnly = !value;
        }

        /// <summary>
        /// Construct a new ListViewCellEditor object and bind it to the given ListView control
        /// </summary>
        /// <param name="lv"></param>
        public ListViewCellEditor(ListViewEx lv)
        {
            mListView = lv;
            mListView.OnScroll += HandleScrollEvent;
            SetupCellEditing();
        }

        /// <summary>
        /// Create default cell editor (TextBox)
        /// and wire up our ListView to the necessary handlers
        /// </summary>
        private void SetupCellEditing()
        {
            mListView.MouseDown += HandleLVMouseDown;
            mListView.DoubleClick += HandleLVDoubleClick;

            var editBox = new TextBox();
            InitializeEditingTextBox(editBox);
            mDefaultCellEditor = editBox;
        }

        /// <summary>
        /// Add a new ComboBox cell editor
        /// </summary>
        /// <param name="choices">Choices in the pick list</param>
        /// <param name="columns">List of column indexes to use this picker for</param>
        public void AddPicker(string[] choices, int[] columns)
        {
            var cmbBox = new ComboBox();
            InitializeEditingComboBox(cmbBox);
            var pickerIdx = mPickers.Count;
            mPickers.Add(cmbBox);
            cmbBox.Items.AddRange(choices);
            foreach (var col in columns)
            {
                mColumnPickers[col] = pickerIdx;
            }
        }

        // ComboBox Cell Editor Stuff

        private void InitializeEditingComboBox(ComboBox cmbBox)
        {
            mListView.Controls.AddRange(new Control[] { cmbBox });
            cmbBox.Size = new Size(0, 0);
            cmbBox.Location = new Point(0, 0);
            cmbBox.SelectedIndexChanged += HandleEditingComboBoxSelectionChanged;
            cmbBox.LostFocus += HandleEditingComboBoxLostFocus;
            cmbBox.KeyPress += HandleEditingComboBoxKeyPress;
            // cmbBox.BackColor = Color.SkyBlue;
            cmbBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBox.Hide();
        }

        private void HandleEditingComboBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 || e.KeyChar == 27)
            {
                var cellEditor = sender as ComboBox;
                cellEditor?.Hide();
            }
        }

        private void HandleEditingComboBoxSelectionChanged(object sender, EventArgs e)
        {
            if (sender is not ComboBox cellEditor) return;

            var sel = cellEditor.SelectedIndex;
            if (sel < 0) return;

            var itemSel = cellEditor.Items[sel].ToString();
            mListViewItemUnderEdit.SubItems[mSubItemUnderEdit].Text = itemSel;
        }

        private void HandleEditingComboBoxLostFocus(object sender, EventArgs e)
        {
            var cellEditor = sender as ComboBox;
            cellEditor?.Hide();
        }

        private void InitializeEditingTextBox(TextBoxBase editBox)
        {
            mListView.Controls.AddRange(new Control[] { editBox });
            editBox.Size = new Size(0, 0);
            editBox.Location = new Point(0, 0);
            editBox.KeyPress += HandleEditTextboxKeyPress;
            editBox.LostFocus += HandleEditingTextBoxLostFocus;
            // editBox.BackColor = Color.LightYellow;
            editBox.BorderStyle = BorderStyle.Fixed3D;
            editBox.Hide();
            editBox.Text = string.Empty;
            editBox.ReadOnly = true;
        }

        private void HandleEditTextboxKeyPress(object sender, KeyPressEventArgs e)
        {
            var cellEditor = sender as TextBox;
            if (e.KeyChar == 13)
            {
                if (cellEditor != null)
                {
                    mListViewItemUnderEdit.SubItems[mSubItemUnderEdit].Text = cellEditor.Text;
                    cellEditor.Hide();
                }
            }
            if (e.KeyChar == 27)
            {
                cellEditor?.Hide();
            }
        }

        private void HandleEditingTextBoxLostFocus(object sender, EventArgs e)
        {
            if (sender is not TextBox cellEditor) return;

            mListViewItemUnderEdit.SubItems[mSubItemUnderEdit].Text = cellEditor.Text;
            cellEditor.Hide(); // mEditBox.Hide();
        }

        /// <summary>
        /// Remember where we clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleLVMouseDown(object sender, MouseEventArgs e)
        {
            mListViewItemUnderEdit = mListView.GetItemAt(e.X, e.Y);
            mLastClickX = e.X;
            mLastClickY = e.Y;
        }

        /// <summary>
        /// Remember where we scrolled horizontally
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleScrollEvent(object sender, ScrollEventArgs e)
        {
            mLastScrollX = e.NewValue;
        }

        /// <summary>
        /// Find the subitem that was clicked and lay the appropriate cell editor over it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleLVDoubleClick(object sender, EventArgs e)
        {
            var cellParams = GetSubitemIndexFromXPos(mLastClickX + mLastScrollX, mListView);
            mSubItemUnderEdit = cellParams[0];
            var cellStartPos = cellParams[1] - mLastScrollX;
            var cellEndPos = cellParams[2] - mLastScrollX;

            Control cellEditor = mDefaultCellEditor;

            if (mColumnPickers.TryGetValue(mSubItemUnderEdit, out var picker))
            {
                cellEditor = mPickers[picker];
            }
            // columnEditors[mSubItemUnderEdit];
            ShowCellEditor(cellStartPos, cellEndPos, mListViewItemUnderEdit, mSubItemUnderEdit, cellEditor);
        }

        /// <summary>
        /// Find the cell in the given ListView that was clicked
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="lv"></param>
        private static int[] GetSubitemIndexFromXPos(int mouseX, ListView lv)
        {
            var results = new int[3];
            var cellStartPos = 0;
            var cellEndPos = 0;
            for (var i = 0; i < lv.Columns.Count; i++)
            {
                var cellWidth = lv.Columns[i].Width;
                cellEndPos = cellStartPos + cellWidth;
                if (mouseX > cellStartPos && mouseX < cellEndPos)
                {
                    results[0] = i;
                    break;
                }
                cellStartPos += cellWidth;
            }
            results[1] = cellStartPos;
            results[2] = cellEndPos;
            return results;
        }

        /// <summary>
        /// Place the cell editor over the cell being edited,
        /// initialize it with the current value of the cell,
        /// and make it visible
        /// </summary>
        /// <param name="cellStartPos"></param>
        /// <param name="cellEndPos"></param>
        /// <param name="lvi"></param>
        /// <param name="subItemIdx"></param>
        /// <param name="cellEditor"></param>
        private static void ShowCellEditor(int cellStartPos, int cellEndPos, ListViewItem lvi, int subItemIdx, Control cellEditor)
        {
            // var r = new Rectangle(cellStartPos, lvi.Bounds.Y, cellEndPos, lvi.Bounds.Bottom);
            cellEditor.Size = new Size(cellEndPos - cellStartPos, lvi.Bounds.Bottom - lvi.Bounds.Top);
            cellEditor.Location = new Point(cellStartPos, lvi.Bounds.Y);
            cellEditor.Show();
            cellEditor.Text = lvi.SubItems[subItemIdx].Text;
            cellEditor.Focus();
        }
    }
}
