﻿using System;
using System.Collections.Generic;
using System.IO;
using Mage;

namespace MageConcatenator
{
    internal class clsFileCombiner
    {
        // Ignore Spelling: Cancelled

        public const int MAX_ROWS_TO_TRACK = 100000;

        private bool mCancelProcessingRequested;

        public bool AddFileNameFirstColumn { get; set; }

        public event EventHandler<MageStatusEventArgs> OnRunCompleted;

        public event EventHandler<MageStatusEventArgs> OnError;

        public event EventHandler<MageStatusEventArgs> OnWarning;

        public event EventHandler<MageStatusEventArgs> OnStatusUpdate;

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
                var filesProcessed = 0;

                using (
                    var writer =
                        new StreamWriter(new FileStream(fiTargetFile.FullName, FileMode.Create, FileAccess.Write,
                                                        FileShare.Read)))
                {
                    var headerWritten = false;
                    var headerLine = string.Empty;

                    var headerDelimiter = ' ';

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

                        var sourceFileName = fiFile.Name;

                        var delimiter = GetDelimiter(fiFile);
                        if (headerDelimiter == ' ')
                        {
                            headerDelimiter = delimiter;
                        }
                        else
                        {
                            if (headerDelimiter != delimiter)
                            {
                                ReportWarning("First file has column delimiter '" + headerDelimiter + "' but file " +
                                              filesProcessed + " has delimiter '" + delimiter + "'");
                            }
                        }

                        var fileSizeBytes = fiFile.Length;

                        using var reader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                        var headerParsed = false;
                        var rowCount = 0;
                        long bytesRead = 0;

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

                if (mCancelProcessingRequested)
                {
                    var cancelMessage = "Cancelled the operation (" + filesProcessed + " file";
                    if (filesProcessed != 1)
                        cancelMessage += "s";
                    cancelMessage += " processed so far)";

                    ReportProcessingComplete(cancelMessage);
                    return false;
                }

                ReportProcessingComplete("Combined " + filesProcessed + " files to create " + fiTargetFile.FullName);
                return true;
            }
            catch (Exception ex)
            {
                ReportError("Error in CombineFiles: " + ex.Message);
                return false;
            }
        }

        private char GetDelimiter(FileSystemInfo fiFile)
        {
            var delimiter = '\t';
            if (string.Equals(fiFile.Extension, ".csv", StringComparison.OrdinalIgnoreCase))
                delimiter = ',';

            return delimiter;
        }

        private void ParseHeaderLine(string currentRow, TextWriter writer, string filePath,
                                     ref bool headerWritten, ref string headerLine, ref char headerDelimiter)
        {
            bool writeHeader;
            string currentHeader;

            if (!headerWritten)
            {
                headerWritten = true;
                headerLine = string.Copy(currentRow);
                currentHeader = headerLine;

                headerDelimiter = VerifyDelimiter(currentRow, headerDelimiter);

                writeHeader = true;
            }
            else
            {
                // Compare this file's header to the first file's header
                if (headerLine.Equals(currentRow, StringComparison.OrdinalIgnoreCase))
                {
                    writeHeader = false;
                    currentHeader = string.Empty;
                }
                else
                {
                    ReportWarning("The header line for file " + Path.GetFileName(filePath) +
                                  " does not match the header line of the first file");

                    // Since the headers don't match, include the new header in the output file
                    writeHeader = true;
                    currentHeader = currentRow;
                }
            }

            if (writeHeader)
            {
                if (AddFileNameFirstColumn)
                    writer.WriteLine("Source_file" + headerDelimiter + currentHeader);
                else
                    writer.WriteLine(currentHeader);
            }
        }

        public bool UpdateFileRowColCounts(List<InputFileInfo> lstFiles)
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

        private void UpdateColCounts(InputFileInfo item)
        {
            try
            {
                var fiFile = new FileInfo(item.FullPath);
                if (!fiFile.Exists)
                {
                    ReportError("File not found: " + item.FullPath);
                    return;
                }

                var delimiter = GetDelimiter(fiFile);

                using var reader = new StreamReader(new FileStream(fiFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                var rowCount = 0;
                var colCount = -1;

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
            catch (Exception ex)
            {
                ReportError("Error in UpdateColCounts for " + item.FullPath + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Examine the row to verify if it is delimited using the specified delimiter (should be tab or comma)
        /// </summary>
        /// <remarks>If we only find one column when we split the row on delimiter, then try the alternate delimiter</remarks>
        /// <param name="currentRow"></param>
        /// <param name="delimiter"></param>
        private char VerifyDelimiter(string currentRow, char delimiter)
        {
            if (string.IsNullOrWhiteSpace(currentRow))
                return delimiter;

            var columns = currentRow.Split(delimiter);

            if (columns.Length == 1)
            {
                var alternateDelimiter = ',';
                if (delimiter == ',')
                    alternateDelimiter = '\t';

                columns = currentRow.Split(alternateDelimiter);
                if (columns.Length > 1)
                {
                    return alternateDelimiter;
                }
            }

            return delimiter;
        }

        protected void ReportProcessingComplete(string message)
        {
            OnRunCompleted?.Invoke(this, new MageStatusEventArgs(message));
        }

        protected void ReportError(string message)
        {
            OnError?.Invoke(this, new MageStatusEventArgs(message));
        }

        private void ReportStatus(string message)
        {
            OnStatusUpdate?.Invoke(this, new MageStatusEventArgs(message));
        }

        private void ReportWarning(string message)
        {
            OnWarning?.Invoke(this, new MageStatusEventArgs(message));
        }
    }
}
