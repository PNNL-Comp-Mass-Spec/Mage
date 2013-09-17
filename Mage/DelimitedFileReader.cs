using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Mage {

    /// <summary>
    /// Mage module that reads content of a delimited files
    /// and streams it to Mage standard tabular ouput
    /// </summary>
    public class DelimitedFileReader : BaseModule, IDisposable {

        #region Member Variables

        private StreamReader mFileReader = null;

        private bool doHeaderLine = true;

        #endregion

        #region Properties

        /// <summary>
        /// delimiter for input file (default to tab)
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// full path to input files
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// whether or not input file has a header line ("Yes" or "No")
        /// </summary>
        public string Header { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new Mage delimited file reader object
        /// (defaulted to "AutoSense" and expecting a header line
        /// </summary>
        public DelimitedFileReader() {
            Delimiter = "AutoSense"; // "\t";
            Header = "Yes";
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// dispose of held resources
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// dispose of held resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            if (mFileReader != null) {
                mFileReader.Dispose();
            }

            //           isDisposed = true;
        }

        #endregion


        #region IBaseModule Members

        /// <summary>
        /// called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare() {
        }

        /// <summary>
        /// called after pipeline run is complete - module can do any special cleanup
        /// this module closes the input file
        /// (override of base class)
        /// </summary>
        public override void Cleanup() {
            base.Cleanup();
            if (mFileReader != null) {
                mFileReader.Close();
            }
        }

        /// <summary>
        /// Pass execution to module instead of having it respond to standard tabular input stream events
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state) {
            UpdateStatus("Reading file " + FilePath);
            doHeaderLine = (Header == "Yes");
            switch (Delimiter) {
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
        /// output contents of file, automatically deciding 
        /// whether it is tab-delimited or comma-delimited
        /// </summary>
        private void OutputContents() {
            bool checkDelimiter = true;
            bool tabDelimited = true;
            char[] delim = "\t".ToCharArray();

			// This RegEx is used to parse CSV files
			// It assures that we only split on commas that are not inside double-quoted strings
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] fields;

            try {
                mFileReader = new StreamReader(new System.IO.FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read));
                string line;
                while ((line = mFileReader.ReadLine()) != null) {
                    if (Abort) {
                        ReportProcessingAborted();
                        break;
                    }

                    // check first line for delimiter type
                    if (checkDelimiter) {
                        tabDelimited = !SwitchToCSV(line);
                        checkDelimiter = false;
                    }

                    // parse line according to delimiter type
                    if (tabDelimited) {
                        fields = line.Split(delim);
                    } else {
                        fields = r.Split(line);
                    }

                    // output line
                    if (doHeaderLine) {
                        doHeaderLine = false;
                        OutputHeaderLine(fields);
                    } else {
                        OutputDataLine(fields);
                    }
                }
                OutputDataLine(null);
                mFileReader.Close();
            } catch (IOException ex) {
                throw new MageException("Cannot access " + System.IO.Path.GetFileName(FilePath) + ": " + ex.Message, ex);
            }
        }

        private static bool SwitchToCSV(string line) {
            bool tabs = line.Contains("\t");
            bool commas = line.Contains(",");
            return commas && !tabs;
        }


        private void OutputFileContents() {
            char[] delim = Delimiter.ToCharArray();
            mFileReader = new StreamReader(FilePath);
            string line;
            while ((line = mFileReader.ReadLine()) != null) {
                if (Abort) {
                    ReportProcessingAborted();
                    break;
                }
                string[] fields = line.Split(delim);
                if (doHeaderLine) {
                    doHeaderLine = false;
                    OutputHeaderLine(fields);
                } else {
                    OutputDataLine(fields);
                }
            }
            OutputDataLine(null);
            mFileReader.Close();
        }

        private void OutputFileContentsFromCSV() {
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            mFileReader = new StreamReader(FilePath);
            string line;
            while ((line = mFileReader.ReadLine()) != null) {
                if (Abort) {
                    ReportProcessingAborted();
                    break;
                }
                string[] fields = r.Split(line);
                if (doHeaderLine) {
                    doHeaderLine = false;
                    OutputHeaderLine(fields);
                } else {
                    OutputDataLine(fields);
                }
            }
            OutputDataLine(null);
            mFileReader.Close();
        }

        private void OutputHeaderLine(string[] fields) {
            // output the column definitions
            List<MageColumnDef> colDefs = new List<MageColumnDef>();
            foreach (string field in fields) {
                colDefs.Add(new MageColumnDef(field, "text", "10"));
            }
            OnColumnDefAvailable(new MageColumnEventArgs(colDefs.ToArray()));
        }

        private void OutputDataLine(string[] fields) {
            OnDataRowAvailable(new MageDataEventArgs(fields));
        }

        private void UpdateStatus(string message) {
            OnStatusMessageUpdated(new MageStatusEventArgs(message));
        }

        #endregion
    }
}
