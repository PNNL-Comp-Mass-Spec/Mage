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
        /// Get complete contents of text display
        /// </summary>
        public string Contents
        {
            get => MainTextCtl.Text;
            set => MainTextCtl.Text = value;
        }

        /// <summary>
        /// Get contents of text display as list of lines
        /// </summary>
        public Collection<string> Lines => new Collection<string>(MainTextCtl.Lines);

        /// <summary>
        /// Control whether text can be edited
        /// </summary>
        public bool ReadOnly
        {
            get => MainTextCtl.ReadOnly;
            set => MainTextCtl.ReadOnly = value;
        }

        /// <summary>
        /// Control which scrollbars are visible
        /// </summary>
        public ScrollBars ScrollBars
        {
            get => MainTextCtl.ScrollBars;
            set => MainTextCtl.ScrollBars = value;
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
        /// Construct new Mage TextDisplayControl object
        /// </summary>
        public TextDisplayControl()
        {
            InitializeComponent();
        }
    }
}
