using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace MageDisplayLib
{

    /// <summary>
    /// Very simple multiline text display.
    /// Exists to be a target for TDPipelineSource class.
    /// </summary>
    public partial class TextDisplayControl : UserControl
    {

        #region Properties

        /// <summary>
        /// get complete contents of text display
        /// </summary>
        public string Contents
        {
            get { return MainTextCtl.Text; }
            set { MainTextCtl.Text = value; }
        }

        /// <summary>
        /// get contents of text display as list of lines
        /// </summary>
        public Collection<string> Lines
        {
            get { return new Collection<string>(MainTextCtl.Lines); }
        }

        /// <summary>
        /// Control whether text can be edited
        /// </summary>
        public bool ReadOnly
        {
            get { return MainTextCtl.ReadOnly; }
            set { MainTextCtl.ReadOnly = value; }
        }

        /// <summary>
        /// Control which scrollbars are visible
        /// </summary>
        public ScrollBars ScrollBars
        {
            get { return MainTextCtl.ScrollBars; }
            set { MainTextCtl.ScrollBars = value; }
        }

        #endregion

        /// <summary>
        /// Set lines of text to display in the control
        /// </summary>
        /// <param name="lines">Lines of text to display</param>
        public void SetLines(string[] lines)
        {
            MainTextCtl.Lines = lines;
        }

        /// <summary>
        /// construct new Mage TextDisplayControl object
        /// </summary>
        public TextDisplayControl()
        {
            InitializeComponent();
        }
    }
}
