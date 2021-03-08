using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Mage
{
    /// <summary>
    /// Mage module that reads content of a delimited files
    /// and streams it to Mage standard tabular output
    /// </summary>
    public class DelimitedFileReader : FileProcessingBase
    {
        #region Member Variables

        private bool doHeaderLine = true;

        #endregion

        #region Properties

        /// <summary>
        /// Delimiter for input file (default to tab)
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Full path to input files
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Whether or not input file has a header line ("Yes" or "No")
        /// </summary>
        public string Header { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Mage delimited file reader object
        /// (defaulted to "AutoSense" and expecting a header line
        /// </summary>
        public DelimitedFileReader()
        {
            Delimiter = "AutoSense"; // "\t";
            Header = "Yes";
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
        }

        /// <summary>
        /// Pass execution to module instead of having it respond to standard tabular input stream events
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state)
        {
            UpdateStatus("Reading file " + PRISM.FileTools.CompactPathString(FilePath, 60));
            doHeaderLine = OptionEnabled(Header);
            switch (Delimiter)
            {
                case "AutoSense":
                    OutputContents();
                    break;
                case "CSV":
                    OutputFileContentsFromCSV();
                    break;
                default:
                    OutputFileContents();
                    break;
            }
        }

        #endregion

        #region Support Functions

        /// <summary>
        /// Output contents of file, automatically deciding
        /// whether it is tab-delimited or comma-delimited
        /// </summary>
        private void OutputContents()
        {
            var checkDelimiter = true;
            var tabDelimited = true;
            var delimiter = "\t".ToCharArray();

            // This RegEx is used to parse CSV files
            // It assures that we only split on commas that are not inside double-quoted strings
            var r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            var delimitedFilePathLocal = DownloadFileIfRequired(FilePath);

            var downloadedMyEMSLFile = delimitedFilePathLocal != FilePath;

            try
            {
                using (var fileReader = new StreamReader(new FileStream(delimitedFilePathLocal, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    string line;
                    while ((line = fileReader.ReadLine()) != null)
                    {
                        if (Abort)
                        {
                            ReportProcessingAborted();
                            break;
                        }

                        // Check first line for delimiter type
                        if (checkDelimiter)
                        {
                            tabDelimited = !SwitchToCSV(line);
                            checkDelimiter = false;
                        }

                        // Parse line according to delimiter type
                        string[] fields;
                        if (tabDelimited)
                        {
                            fields = line.Split(delimiter);
                        }
                        else
                        {
                            fields = r.Split(line);
                        }

                        // Output line
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

                if (downloadedMyEMSLFile)
                    DeleteFileIfLowDiskSpace(delimitedFilePathLocal);
            }
            catch (IOException e)
            {
                var errorMessage = "Cannot access " + Path.GetFileName(delimitedFilePathLocal) + ": " + e.Message;
                var ex = ReportMageException(errorMessage, e);
                throw ex;
            }
        }

        private static bool SwitchToCSV(string line)
        {
            var tabs = line.Contains("\t");
            var commas = line.Contains(",");
            return commas && !tabs;
        }

        private void OutputFileContents()
        {
            var delimiter = Delimiter.ToCharArray();

            using (var fileReader = new StreamReader(new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    if (Abort)
                    {
                        ReportProcessingAborted();
                        break;
                    }
                    var fields = line.Split(delimiter);
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
        }

        private void OutputFileContentsFromCSV()
        {
            var r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            using (var fileReader = new StreamReader(new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null)
                {
                    if (Abort)
                    {
                        ReportProcessingAborted();
                        break;
                    }
                    var fields = r.Split(line);
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
        }

        private void OutputHeaderLine(IEnumerable<string> fields)
        {
            // Output the column definitions
            var colDefs = new List<MageColumnDef>();
            foreach (var field in fields)
            {
                colDefs.Add(new MageColumnDef(field, "text", "10"));
            }
            OnColumnDefAvailable(new MageColumnEventArgs(colDefs.ToArray()));
        }

        private void OutputDataLine(string[] fields)
        {
            OnDataRowAvailable(new MageDataEventArgs(fields));
        }

        private void UpdateStatus(string message)
        {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        #endregion
    }
}
