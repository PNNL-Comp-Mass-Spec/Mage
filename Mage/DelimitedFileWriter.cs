using System;
using System.Collections.Generic;
using System.IO;

namespace Mage
{
    /// <summary>
    /// Mage module that writes that data that it receives via its
    /// Mage standard tabular input to a delimited text file
    /// </summary>
    public class DelimitedFileWriter : BaseModule, IDisposable
    {
        // Ignore Spelling: Mage

        #region Member Variables

        private StreamWriter mOutFile;

        private bool mAppendFlag;

        #endregion

        #region Properties

        /// <summary>
        /// Delimiter to use for output file (defaults to tab)
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Full path for output files
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Set to "Yes", "True", or "1" to include a header line in the output file
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Whether or not output file should be overwritten or appended to
        /// if it exists ("Yes" or "No")
        /// </summary>
        public string Append { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct new Mage delimited file writer module
        /// with property defaults:
        /// Delimiter = "\t";
        /// Header = "Yes";
        /// Append = "No";
        /// </summary>
        public DelimitedFileWriter()
        {
            Delimiter = "\t";
            Header = "Yes";
            Append = "No";
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of held resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of held resources
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to dispose the managed resources of the class
            }

            // Code to dispose the unmanaged resources of the class
            mOutFile?.Dispose();

            // isDisposed = true;
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
            {
                const string errorMessage = "FilePath must be defined before calling Prepare in DelimitedFileWriter";
                var ex = ReportMageException(errorMessage);
                throw ex;
            }

            var dirPath = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (OptionEnabled(Append))
            {
                mAppendFlag = File.Exists(FilePath);
            }
            var ext = Path.GetExtension(FilePath).ToLower();
            if (ext == ".csv")
            {
                Delimiter = ",";
            }
            try
            {
                mOutFile = new StreamWriter(FilePath, mAppendFlag);
            }
            catch (Exception e)
            {
                var errorMessage = "Error initializing file " + FilePath + ": " + e.Message;
                var ex = ReportMageException(errorMessage, e);
                throw ex;
            }
        }

        /// <summary>
        /// Called after pipeline run is complete - module can do any special cleanup
        /// this module closes the output file
        /// (override of base class)
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();
            mOutFile?.Close();
        }

        /// <summary>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            if (OptionEnabled(Header) && !mAppendFlag)
            {
                OutputHeader();
            }
        }

        /// <summary>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                OutputDataRow(args.Fields);
            }
            else
            {
                mOutFile.Close();
            }
        }

        #endregion

        #region Support Functions

        private void OutputHeader()
        {
            var h = new List<string>();
            // Use our output column definitions, if we have them
            // Otherwise just use the input column definitions
            if (OutputColumnDefs != null)
            {
                foreach (var col in OutputColumnDefs)
                {
                    h.Add(col.Name);
                }
            }
            else
            {
                foreach (var col in InputColumnDefs)
                {
                    h.Add(col.Name);
                }
            }
            mOutFile.WriteLine(string.Join(Delimiter, h));
        }

        private void OutputDataRow(string[] vals)
        {
            var delimiter = string.Empty;

            // Remap results according to our output column definitions, if we have them
            // Otherwise just use the as-delivered format
            var outRow = vals;
            if (OutputColumnDefs != null)
            {
                outRow = MapDataRow(vals);
            }

            foreach (var item in outRow)
            {
                mOutFile.Write(delimiter + (item ?? string.Empty));
                delimiter = Delimiter;
            }
            mOutFile.WriteLine();
        }

        #endregion
    }
}
