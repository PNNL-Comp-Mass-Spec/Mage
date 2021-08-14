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
        // Ignore Spelling: Mage

        // Object whose data we are serving
        private readonly TextDisplayControl myTextControl;

        // Delimiter for parsing text into tabular format
        private char[] mDelimiter = { '\t' };

        private bool doHeaderLine = true;

        /// <summary>
        /// Construct new Mage TDPipelineSource object
        /// that can serve data from given TextDisplayControl object
        /// via Mage standard tabular output stream
        /// </summary>
        /// <param name="lc"></param>
        public TDPipelineSource(TextDisplayControl lc)
        {
            myTextControl = lc;
            Header = "Yes";
        }

        /// <summary>
        /// Delimiter used to separate text lines into fields
        /// </summary>
        public string Delimiter
        {
            get => mDelimiter.ToString();
            set => mDelimiter = value.ToCharArray();
        }

        /// <summary>
        /// There is a header line ("Yes" or "No")
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// Output each row in associated TextDisplayList object
        /// to Mage standard tabular output, one row at a time.
        /// (override of base class)
        /// </summary>
        public override void Run(object state)
        {
            doHeaderLine = OptionEnabled(Header);
            OutputRowsFromList();
        }

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

        private void OutputHeaderLine(IEnumerable<string> fields)
        {
            // Output the column definitions
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
    }
}
