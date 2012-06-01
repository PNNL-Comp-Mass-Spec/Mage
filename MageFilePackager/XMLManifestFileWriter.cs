using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mage;

namespace MageFilePackager {

    class XMLManifestFileWriter : BaseModule, IDisposable {

        #region Member Variables

        private StreamWriter _mOutFile;

        #endregion

        #region Properties

        /// <summary>
        /// full path for output files
        /// </summary>
        public string FilePath { get; set; }

        public Dictionary<string, string> Prefixes { get; set; }

        #endregion

        #region Constructors

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
            if (_mOutFile != null) {
                _mOutFile.Dispose();
            }

            //            isDisposed = true;
        }

        #endregion


        #region IBaseModule Members

        /// <summary>
        /// called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare() {
            string dirPath = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            try {
                _mOutFile = new StreamWriter(FilePath);
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
            if (_mOutFile != null) {
                _mOutFile.Close();
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
            OutputHeader();
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
                OutputFooter();
                _mOutFile.Close();
            }
        }

        #endregion

        #region Support Functions

        private void OutputHeader() {
            // Opportunity to output opening XML elements
            //            _mOutFile.WriteLine("<?xml version='1.0' encoding='utf-8'?>");
            //            _mOutFile.WriteLine("<paths>");
            if (Prefixes != null) {
                foreach (KeyValuePair<string, string> kv in Prefixes) {
                    _mOutFile.WriteLine(string.Format("<prefix source='{0}' value='{1}' />", kv.Key, kv.Value));
                }
            }
        }

        private void OutputFooter() {
            // opportunity to write any closing XML elements
            //            _mOutFile.WriteLine("</paths>");
        }

        private void OutputDataRow(object[] vals) {
            var sb = new StringBuilder();
            sb.Append("<path ");
            for (int i = 0; i < InputColumnDefs.Count; i++) {
                sb.Append(string.Format("{0}='{1}' ", InputColumnDefs[i].Name, vals[i]));
            }
            sb.Append(" />");
            _mOutFile.WriteLine(sb.ToString());
        }

        #endregion
    }
}
