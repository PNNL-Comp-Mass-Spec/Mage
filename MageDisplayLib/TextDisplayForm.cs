using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MageDisplayLib {
	
	/// <summary>
	/// Displays a textbox
	/// </summary>
	public partial class TextDisplayForm : Form {

		/// <summary>
		/// get complete contents of text display
		/// </summary>
		public string Contents {
			get { return textDisplayControl1.Text; }
			set { textDisplayControl1.Text = value; }
		}

		/// <summary>
		/// Control whether text can be edited
		/// </summary>
		public bool ReadOnly {
			get { return textDisplayControl1.ReadOnly; }
			set { textDisplayControl1.ReadOnly = value; }
		}

		/// <summary>
		/// Set lines of text to display in the control
		/// </summary>
		/// <param name="lines">Lines of text to display</param>
		public void SetLines(string[] lines) {
			textDisplayControl1.SetLines(lines);
		}

		/// <summary>
		/// Control which scrollbars are visible
		/// </summary>
		public System.Windows.Forms.ScrollBars ScrollBars {
			get { return textDisplayControl1.ScrollBars; }
			set { textDisplayControl1.ScrollBars = value; }
		}

		/// <summary>
		/// Construct a new TextDisplayForm UI component
		/// </summary>
		public TextDisplayForm() {
			InitializeComponent();
		}

		private void CloseCtl_Click(object sender, EventArgs e) {
			this.Dispose();
		}
	}
}
