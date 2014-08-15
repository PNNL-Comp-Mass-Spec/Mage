using System;
using System.Collections.Generic;
using System.IO;

namespace Mage {

    /// <summary>
    /// Mage module that writes that data that it receives via its 
    /// Mage standard tabular input to a delimited text file
    /// </summary>
    public class DelimitedFileWriter : BaseModule, IDisposable {

        #region Member Variables

        private StreamWriter mOutFile;

        bool mAppendFlag;

        #endregion

        #region Properties

        /// <summary>
        /// Delimiter to use for ouput file (defaults to tab)
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// full path for output files
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// whether or not output file should be overwritten or appended to 
        /// if it exists ("Yes" or "No")
        /// </summary>
        public string Append { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// construct new Mage delimited file writer module
        /// with propery defaults: 
        /// Delimiter = "\t";
        /// Header = "Yes";
        /// Append = "No";
        /// </summary>
        public DelimitedFileWriter() {
            Delimiter = "\t";
            Header = "Yes";
            Append = "No";
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
        private void Dispose(bool disposing) {
            if (disposing) {
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            if (mOutFile != null) {
                mOutFile.Dispose();
            }

            //            isDisposed = true;
        }

        #endregion


        #region IBaseModule Members

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
	        if (string.IsNullOrWhiteSpace(FilePath))
		        throw new MageException("FilePath must be defined before calling Prepare in DelimitedFileWriter");

            string dirPath = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            if (Append == "Yes") {
                mAppendFlag = File.Exists(FilePath);
            }
            string ext = Path.GetExtension(FilePath).ToLower();
            if (ext == ".csv") {
                Delimiter = ",";
            }
			try {
				mOutFile = new StreamWriter(FilePath, mAppendFlag);
			} catch (Exception ex) {
				throw new MageException("Error initializing file " + FilePath + ": " + ex.Message);
			}
            
        }

        /// <summary>
        /// called after pipeline run is complete - module can do any special cleanup
        /// this module closes the output file
        /// (override of base class)
        /// </summary>
        public override void Cleanup() {
            base.Cleanup();
            if (mOutFile != null) {
                mOutFile.Close();
            }
        }

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args) {
            base.HandleColumnDef(sender, args);
            if (Header == "Yes" && !mAppendFlag) {
                OutputHeader();
            }
        }

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args) {
            if (args.DataAvailable) {
                OutputDataRow(args.Fields);
            } else {
                mOutFile.Close();
            }
        }

        #endregion

        #region Support Functions

        private void OutputHeader() {
            var h = new List<string>();
            // use our output column definitions, if we have them
            // otherwise just use the input column definitions
            if (OutputColumnDefs != null) {
                foreach (MageColumnDef col in OutputColumnDefs) {
                    h.Add(col.Name);
                }
            } else {
                foreach (MageColumnDef col in InputColumnDefs) {
                    h.Add(col.Name);
                }
            }
            mOutFile.WriteLine(string.Join(Delimiter, h.ToArray()));
        }

		private void OutputDataRow(string[] vals)
		{
            string delim = "";
            // remap results according to our output column definitions, if we have them
            // otherwise just use the as-delivered format
			string[] outRow = vals;
            if (OutputColumnDefs != null) {
                outRow = MapDataRow(vals);
            }
            foreach (var item in outRow) {
				mOutFile.Write(delim + (item ?? ""));
                delim = Delimiter;
            }
            mOutFile.WriteLine();
        }

        #endregion
    }
}
