using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MageDisplayLib
{

    /// <summary>
    /// implements a floating entry field editor that overlays display cells
    /// </summary>
    public class ListViewCellEditor
    {

        #region Member Variables

        /// <summary>
        /// default cell editor
        /// </summary>
        private TextBox mDefaultCellEditor = null;

        /// <summary>
        /// the ListView control that we provide cell editing for
        /// </summary>
        private ListViewEx mListView = null;

        /// <summary>
        /// The ListViewItem representing the row 
        /// containing the cell currently being edited
        /// </summary>
        private ListViewItem mListViewItemUnderEdit;

        /// <summary>
        /// The columm index of the cell currently being edited
        /// </summary>
        private int mSubItemUnderEdit = 0;

        /// <summary>
        /// position of last mouse click (used to determine which cell to edit)
        /// </summary>
        private int mLastClickX = 0;
        private int mLastClickY = 0;

        /// <summary>
        /// position of last horizontal scroll
        /// </summary>
        private int mLastScrollX = 0;

        /// <summary>
        /// List of ComboBox cell editors
        /// </summary>
        private List<Control> mPickers = new List<Control>();

        /// <summary>
        /// Association between column index and its ComboBox cell editor
        /// </summary>
        private Dictionary<int, int> mColumnPickers = new Dictionary<int, int>();

        #endregion

        #region Properties

        /// <summary>
        /// defines whether or not the cell editor 
        /// can make changes to underlying cell
        /// </summary>
        public bool Editable
        {
            get { return mDefaultCellEditor.ReadOnly; }
            set { mDefaultCellEditor.ReadOnly = !value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new ListViewCellEditor object and bind it to the given ListView control
        /// </summary>
        /// <param name="lv"></param>
        public ListViewCellEditor(ListViewEx lv)
        {
            mListView = lv;
            mListView.onScroll += HandleScrollEvent;
            SetupCellEditing();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Create default cell editor (TextBox) 
        /// and wire up our ListView to the necessary handlers
        /// </summary>
        private void SetupCellEditing()
        {
            mListView.MouseDown += new MouseEventHandler(this.HandleLVMouseDown);
            mListView.DoubleClick += new EventHandler(this.HandleLVDoubleClick);

            TextBox editBox = new TextBox();
            InitializeEditingTextBox(editBox);
            mDefaultCellEditor = editBox;
        }

        /// <summary>
        /// Add a new ComboBox cell editor
        /// </summary>
        /// <param name="choices">Choices in the picklis</param>
        /// <param name="columns">List of column indexes to use this picker for</param>
        public void AddPicker(string[] choices, int[] columns)
        {
            ComboBox cmbBox = new ComboBox();
            InitializeEditingComboBox(cmbBox);
            int pickerIdx = mPickers.Count;
            mPickers.Add(cmbBox);
            cmbBox.Items.AddRange(choices);
            foreach (int col in columns)
            {
                mColumnPickers[col] = pickerIdx;
            }
        }

        #endregion

        #region ComboBox Cell Editor Stuff

        private void InitializeEditingComboBox(ComboBox cmbBox)
        {
            mListView.Controls.AddRange(new Control[] { cmbBox });
            cmbBox.Size = new Size(0, 0);
            cmbBox.Location = new Point(0, 0);
            cmbBox.SelectedIndexChanged += new EventHandler(this.HandleEditingComboBoxSelectionChanged);
            cmbBox.LostFocus += new EventHandler(this.HandleEditingComboBoxLostFocus);
            cmbBox.KeyPress += new KeyPressEventHandler(this.HandleEditingComboBoxKeyPress);
            //            cmbBox.BackColor = Color.SkyBlue;
            cmbBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBox.Hide();
        }

        private void HandleEditingComboBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 || e.KeyChar == 27)
            {
                ComboBox cellEditor = sender as ComboBox;
                cellEditor.Hide();
            }
        }

        private void HandleEditingComboBoxSelectionChanged(object sender, EventArgs e)
        {
            ComboBox cellEditor = sender as ComboBox;
            int sel = cellEditor.SelectedIndex;
            if (sel >= 0)
            {
                string itemSel = cellEditor.Items[sel].ToString();
                mListViewItemUnderEdit.SubItems[mSubItemUnderEdit].Text = itemSel;
            }
        }

        private void HandleEditingComboBoxLostFocus(object sender, EventArgs e)
        {
            ComboBox cellEditor = sender as ComboBox;
            cellEditor.Hide();
        }

        #endregion

        #region TextBox Cell Editor Stuff

        private void InitializeEditingTextBox(TextBox editBox)
        {
            mListView.Controls.AddRange(new Control[] { editBox });
            editBox.Size = new Size(0, 0);
            editBox.Location = new Point(0, 0);
            editBox.KeyPress += new KeyPressEventHandler(this.HandleEditTextboxKeyPress);
            editBox.LostFocus += new EventHandler(this.HandleEditingTextBoxLostFocus);
            //           editBox.BackColor = Color.LightYellow;
            editBox.BorderStyle = BorderStyle.Fixed3D;
            editBox.Hide();
            editBox.Text = "";
            editBox.ReadOnly = true;
        }

        private void HandleEditTextboxKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox cellEditor = sender as TextBox;
            if (e.KeyChar == 13)
            {
                mListViewItemUnderEdit.SubItems[mSubItemUnderEdit].Text = cellEditor.Text;
                cellEditor.Hide();
            }
            if (e.KeyChar == 27)
                cellEditor.Hide();
        }

        private void HandleEditingTextBoxLostFocus(object sender, EventArgs e)
        {
            TextBox cellEditor = sender as TextBox;
            mListViewItemUnderEdit.SubItems[mSubItemUnderEdit].Text = cellEditor.Text;
            cellEditor.Hide(); //  mEditBox.Hide();
        }

        #endregion

        #region Initiate Cell Editing

        /// <summary>
        /// remember where we clicked
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
        /// remember where we scrolled horizontally
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleScrollEvent(object sender, ScrollEventArgs e)
        {
            mLastScrollX = e.NewValue;
        }

        /// <summary>
        /// find the subitem that was clicked and lay the appropriate cell editor over it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleLVDoubleClick(object sender, EventArgs e)
        {
            int[] cellParams = GetSubitemIndexFromXPos(mLastClickX + mLastScrollX, mListView);
            mSubItemUnderEdit = cellParams[0];
            int cellStartPos = cellParams[1] - mLastScrollX;
            int cellEndPos = cellParams[2] - mLastScrollX;

            Control cellEditor = mDefaultCellEditor;
            if (mColumnPickers.ContainsKey(mSubItemUnderEdit))
            {
                cellEditor = mPickers[mColumnPickers[mSubItemUnderEdit]];
            }
            // columnEditors[mSubItemUnderEdit];
            ShowCellEditor(cellStartPos, cellEndPos, mListViewItemUnderEdit, mSubItemUnderEdit, cellEditor);
        }

        /// <summary>
        /// Find the cell in the given ListView that was clicked
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="lv"></param>
        /// <returns></returns>
        private static int[] GetSubitemIndexFromXPos(int mouseX, ListView lv)
        {
            int[] results = new int[3];
            int cellStartPos = 0;
            int cellEndPos = 0;
            int cellWidth = 0;
            for (int i = 0; i < lv.Columns.Count; i++)
            {
                cellWidth = lv.Columns[i].Width;
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
        /// inialize it with the current value of the cell,
        /// and make it visible
        /// </summary>
        /// <param name="cellStartPos"></param>
        /// <param name="cellEndPos"></param>
        /// <param name="lvi"></param>
        /// <param name="subItemIdx"></param>
        /// <param name="cellEditor"></param>
        private static void ShowCellEditor(int cellStartPos, int cellEndPos, ListViewItem lvi, int subItemIdx, Control cellEditor)
        {
            Rectangle r = new Rectangle(cellStartPos, lvi.Bounds.Y, cellEndPos, lvi.Bounds.Bottom);
            cellEditor.Size = new Size(cellEndPos - cellStartPos, lvi.Bounds.Bottom - lvi.Bounds.Top);
            cellEditor.Location = new Point(cellStartPos, lvi.Bounds.Y);
            cellEditor.Show();
            cellEditor.Text = lvi.SubItems[subItemIdx].Text;
            cellEditor.Focus();
        }

        #endregion

    }

}
