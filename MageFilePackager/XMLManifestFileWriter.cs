using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mage;

namespace MageFilePackager
{
    [Obsolete("Unused")]
    internal class XMLManifestFileWriter : BaseModule, IDisposable
    {
        private StreamWriter _mOutFile;

        public XMLManifestFileWriter(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Full path for output files
        /// </summary>
        public string FilePath { get; }

        public Dictionary<string, string> Prefixes { get; set; }

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
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            _mOutFile?.Dispose();

            // isDisposed = true;
        }

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
            var dirPath = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            try
            {
                _mOutFile = new StreamWriter(FilePath);
            }
            catch (Exception ex)
            {
                throw new MageException("Error initializing file " + FilePath + ": " + ex.Message);
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
            _mOutFile?.Close();
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
            OutputHeader();
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
                OutputFooter();
                _mOutFile.Close();
            }
        }

        // Support methods

        private void OutputHeader()
        {
            // Opportunity to output opening XML elements
            //            _mOutFile.WriteLine("<?xml version='1.0' encoding='utf-8'?>");
            //            _mOutFile.WriteLine("<paths>");
            if (Prefixes != null)
            {
                foreach (var kv in Prefixes)
                {
                    _mOutFile.WriteLine("<prefix source='{0}' value='{1}' />", kv.Key, kv.Value);
                }
            }
        }

        private void OutputFooter()
        {
            // Opportunity to write any closing XML elements
            // _mOutFile.WriteLine("</paths>");
        }

        private void OutputDataRow(IReadOnlyList<string> vals)
        {
            var sb = new StringBuilder();
            sb.Append("<path ");
            for (var i = 0; i < InputColumnDefs.Count; i++)
            {
                sb.Append(string.Format("{0}='{1}' ", InputColumnDefs[i].Name, vals[i]));
            }
            sb.Append(" />");
            _mOutFile.WriteLine(sb.ToString());
        }
    }
}
