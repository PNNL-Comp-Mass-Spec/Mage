using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mage;

namespace MageConcatenator
{
    class clsFileCombiner
    {


        #region Constants and Classwide Variables

        public const int MAX_ROWS_TO_TRACK = 100000;

        private bool mCancelProcessingRequested;

        #endregion

        #region Properties

        public bool AddFileNameFirstColumn { get; set; }

        #endregion

        #region Events

        public event EventHandler<MageStatusEventArgs> OnRunCompleted;

        public event EventHandler<MageStatusEventArgs> OnError;

        public event EventHandler<MageStatusEventArgs> OnWarning;

        public event EventHandler<MageStatusEventArgs> OnStatusUpdate;

        #endregion

        /// <summary>
        /// Cancel the current processing
        /// and set the abort flag to stop the queue
        /// </summary>
        public void Cancel()
        {
            Globals.AbortRequested = true;
            mCancelProcessingRequested = true;
        }

        public bool CombineFiles(List<string> lstFilePaths, string targetFilePath)
        {
            try
            {
                mCancelProcessingRequested = false;

                var fiTargetFile = new FileInfo(targetFilePath);
                int filesProcessed = 0;

                using (
                    var writer =
                        new StreamWriter(new FileStream(fiTargetFile.FullName, FileMode.Create, FileAccess.Write,
                                                        FileShare.Read)))
                {
                    bool headerWritten = false;
                    string headerLine = string.Empty;

                    char headerDelimiter = ' ';

                    foreach (var filePath in lstFilePaths)
                    {
                        if (mCancelProcessingRequested)
                            break;

                        var fiFile = new FileInfo(filePath);
                        if (!fiFile.Exists)
                        {
                            ReportError("File not found: " + filePath);
                            continue;
                        }

                        var percentComplete = filesProcessed / (float)lstFilePaths.Count * 100;
                        var dtLastStatus = DateTime.UtcNow;
                        ReportStatus(percentComplete.ToString("0") + "% complete, processing " + filePath);

                        filesProcessed++;

                        string sourceFileName = fiFile.Name;

                        char delimiter = GetDelimiter(fiFile);
                        if (headerDelimiter == ' ')
                        {
                            headerDelimiter = delimiter;
                        }
                        else
                        {
                            if (headerDelimiter != delimiter)
                                ReportWarning("First file has column delimiter '" + headerDelimiter + "' but file " +
                                              filesProcessed + " has delimiter '" + delimiter + "'");
                        }

                        Int64 fileSizeBytes = fiFile.Length;

                        using (
                            var reader =
                                new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            )
                        {
                            bool headerParsed = false;
                            int rowCount = 0;
                            Int64 bytesRead = 0;

                            while (reader.Peek() > -1)
                            {

                                var currentRow = reader.ReadLine();

                                if (string.IsNullOrWhiteSpace(currentRow))
                                    continue;

                                if (!headerParsed)
                                {
                                    ParseHeaderLine(currentRow, writer, filePath, ref headerWritten, ref headerLine, ref headerDelimiter);
                                    headerParsed = true;
                                    continue;
                                }

                                if (AddFileNameFirstColumn)
                                    writer.WriteLine(sourceFileName + delimiter + currentRow);
                                else
                                    writer.WriteLine(currentRow);

                                rowCount++;
                                bytesRead += currentRow.Length + 1;

                                if (rowCount % 10000 == 0)
                                {
                                    if (mCancelProcessingRequested)
                                        break;

                                    if (DateTime.UtcNow.Subtract(dtLastStatus).TotalSeconds >= 0.5)
                                    {
                                        dtLastStatus = DateTime.UtcNow;
                                        var percentCompleteOverall = percentComplete + bytesRead / (float)fileSizeBytes / lstFilePaths.Count * 100;
                                        ReportStatus(percentCompleteOverall.ToString("0") + "% complete, processing " + filePath);
                                    }
                                }

                            }
                        }
                    }

                }

                if (mCancelProcessingRequested)
                {
                    var cancelMessage = "Cancelled the operation (" + filesProcessed + " file";
                    if (filesProcessed != 1)
                        cancelMessage += "s";
                    cancelMessage += " processed so far)";

                    ReportProcessingComplete(cancelMessage);
                    return false;
                }
                else
                {
                    ReportProcessingComplete("Combined " + filesProcessed + " files to create " + fiTargetFile.FullName);
                    return true;
                }

            }
            catch (Exception ex)
            {
                ReportError("Error in CombineFiles: " + ex.Message);
                return false;
            }
        }


        private char GetDelimiter(FileInfo fiFile)
        {
            char delimiter = '\t';
            if (fiFile.Extension.ToLower() == ".csv")
                delimiter = ',';

            return delimiter;
        }

        private void ParseHeaderLine(string currentRow, StreamWriter writer, string filePath,
                                     ref bool headerWritten, ref string headerLine, ref char headerDelimiter)
        {

            if (!headerWritten)
            {
                headerWritten = true;
                headerLine = string.Copy(currentRow);

                headerDelimiter = VerifyDelimiter(currentRow, headerDelimiter);

                if (AddFileNameFirstColumn)
                    writer.WriteLine("Source_file" + headerDelimiter + headerLine);
                else
                    writer.WriteLine(headerLine);

                return;
            }

            // Compare this file's header to the first file's header
            if (String.Compare(headerLine, currentRow, StringComparison.OrdinalIgnoreCase) != 0)
            {
                ReportWarning("The header line for file " + Path.GetFileName(filePath) +
                          " does not match the header line of the first file");
            }

        }

        public bool UpdateFileRowColCounts(List<clsFileInfo> lstFiles)
        {
            try
            {
                mCancelProcessingRequested = false;

                foreach (var item in lstFiles)
                {
                    UpdateColCounts(item);
                }

                return true;
            }
            catch (Exception ex)
            {
                ReportError("Error in UpdateFileRowColCounts: " + ex.Message);
                return false;
            }
        }

        private bool UpdateColCounts(clsFileInfo item)
        {
            try
            {
                var fiFile = new FileInfo(item.FullPath);
                if (!fiFile.Exists)
                {
                    ReportError("File not found: " + item.FullPath);
                    return false;
                }

                char delimiter = GetDelimiter(fiFile);

                using (var reader = new StreamReader(new FileStream(fiFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    int rowCount = 0;
                    int colCount = -1;

                    while (reader.Peek() > -1)
                    {
                        var currentRow = reader.ReadLine();
                        rowCount++;

                        if (rowCount == MAX_ROWS_TO_TRACK)
                            break;

                        if (string.IsNullOrWhiteSpace(currentRow))
                            continue;

                        if (colCount < 0)
                        {
                            delimiter = VerifyDelimiter(currentRow, delimiter);

                            var columns = currentRow.Split(delimiter);

                            if (columns.Length > 0)
                            {
                                colCount = columns.Length;
                            }

                        }
                    }

                    item.Columns = colCount;
                    item.Rows = rowCount;
                }

                return true;
            }
            catch (Exception ex)
            {
                ReportError("Error in UpdateColCounts for " + item.FullPath + ": " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Examine the row to verify if it is delimited using the specified delimiter (should be tab or comma)
        /// </summary>
        /// <param name="currentRow"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        /// <remarks>If we only find one column when we split the row on delimiter, then try the alternate delimiter</remarks>
        private char VerifyDelimiter(string currentRow, char delimiter)
        {
            if (string.IsNullOrWhiteSpace(currentRow))
                return delimiter;

            var columns = currentRow.Split(delimiter);

            if (columns.Count() == 1)
            {
                char alternateDelimiter = ',';
                if (delimiter == ',')
                    alternateDelimiter = '\t';

                columns = currentRow.Split(alternateDelimiter);
                if (columns.Count() > 1)
                {
                    return alternateDelimiter;
                }
            }

            return delimiter;

        }

        protected void ReportProcessingComplete(string message)
        {
            if (OnRunCompleted != null)
            {
                OnRunCompleted(this, new MageStatusEventArgs(message));
            }
        }

        protected void ReportError(string message)
        {
            if (OnError != null)
            {
                OnError(this, new MageStatusEventArgs(message));
            }
        }

        private void ReportStatus(string message)
        {
            if (OnStatusUpdate != null)
            {
                OnStatusUpdate(this, new MageStatusEventArgs(message));
            }
        }

        private void ReportWarning(string message)
        {
            if (OnWarning != null)
            {
                OnWarning(this, new MageStatusEventArgs(message));
            }
        }

    }
}
