using System.Collections.Generic;
using Mage;

namespace MageDisplayLib
{

    /// <summary>
    /// This is a pipeline module
    /// that can serve the contents of a TextDisplayControl to standard tabular output
    /// </summary>
    public class TDPipelineSource : BaseModule
    {

        #region Member Variables

        // object whose data we are serving
        private readonly TextDisplayControl myTextControl;

        // delimiter for parsing text into tabular format
        private char[] mDelimiter = { '\t' };

        private bool doHeaderLine = true;

        #endregion

        #region Constructors

        /// <summary>
        /// construct new Mage TDPipelineSource object
        /// that can serve data from given TextDisplayControl object
        /// via Mage standard tabular output stream
        /// </summary>
        /// <param name="lc"></param>
        public TDPipelineSource(TextDisplayControl lc)
        {
            myTextControl = lc;
            Header = "Yes";
        }

        #endregion

        #region Properties

        /// <summary>
        /// delimiter used to separate text lines into fields
        /// </summary>
        public string Delimiter
        {
            get => mDelimiter.ToString();
            set => mDelimiter = value.ToCharArray();
        }

        /// <summary>
        /// there is a header line ("Yes" or "No")
        /// </summary>
        public string Header { get; }


        #endregion

        #region IBaseModule Members

        /// output each row in associated TextDisplayList object
        /// to Mage standard tabular output, one row at a time.
        /// (override of base class)
        public override void Run(object state)
        {
            doHeaderLine = OptionEnabled(Header);
            OutputRowsFromList();
        }

        #endregion

        #region Private Functions

        private void OutputRowsFromList()
        {
            foreach (var line in myTextControl.Lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                var fields = line.Split(mDelimiter);
                if (doHeaderLine)
                {
                    doHeaderLine = false;
                    OutputHeaderLine(fields);
                }
                else
                {
                    OutputDataLine(fields);
                }
            }
            OutputDataLine(null);
        }

        private void OutputHeaderLine(string[] fields)
        {
            // output the column definitions
            var columnDefs = new List<MageColumnDef>();
            foreach (var field in fields)
            {
                var colDef = new MageColumnDef(field, "text", "10");
                columnDefs.Add(colDef);
            }
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
        }

        private void OutputDataLine(string[] fields)
        {
            OnDataRowAvailable(new MageDataEventArgs(fields));
        }

        #endregion

    }

}
