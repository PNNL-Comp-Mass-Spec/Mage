﻿using System.Collections.Generic;
using System.IO;

namespace Mage
{
    // Ignore Spelling: Mage, renamer

    /// <summary>
    /// Delegate for a function that returns an output file name for a given input file name and parameters
    /// </summary>
    /// <param name="sourceFile">the original name of the file</param>
    /// <param name="fieldPos">index to the metadata field to be used in renaming</param>
    /// <param name="fields">list of metadata fields for original file</param>
    public delegate string OutputFileNamer(string sourceFile, Dictionary<string, int> fieldPos, string[] fields);

    /// <summary>
    /// <para>
    /// Module that provides base functions for processing one or more input files
    /// </para>
    /// <para>
    /// It expects to receive path information for files via its standard tabular input
    /// </para>
    /// <para>
    /// Each row of standard tabular input will contain information for a single file
    /// (parameters SourceFileColumnName and SourceDirectoryColumnName) define which
    /// columns in standard input contain the directory and name of the input file.
    /// </para>
    /// <para>
    /// The OutputDirectoryPath parameter tells this module where to put results files
    /// </para>
    /// <para>
    /// This module outputs a record of each file processed on standard tabular output
    /// </para>
    /// </summary>
    public class FileContentProcessor : FileProcessingBase
    {
        // Delegate that this module calls to get output file name
        private OutputFileNamer GetOutputFileName;

        // Methods available to clients

        /// <summary>
        /// Define a delegate function that will generate output file name
        /// </summary>
        /// <param name="namer"></param>
        public void SetOutputFileNamer(OutputFileNamer namer)
        {
            GetOutputFileName = namer;
        }

        /// <summary>
        /// Path to the directory into which the
        /// processed input file contents will be saved as an output file
        /// (required by subclasses that create result files)
        /// </summary>
        public string OutputDirectoryPath { get; set; }

        /// <summary>
        /// If this is not blank, it will be combined with the
        /// OutputDirectoryPath to generate the output file.
        /// This will override use of any naming information
        /// from data row
        /// </summary>
        public string OutputFileName { get; set; }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input directory path
        /// (optional - defaults to "Directory"; previously defaulted to "Folder")
        /// </summary>
        public string SourceDirectoryColumnName { get; set; }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input file name
        /// (optional - defaults to "File")
        /// </summary>
        public string SourceFileColumnName { get; set; }

        /// <summary>
        /// Name of the column in the standard tabular input
        /// that contains the input file type
        /// (optional - defaults to blank)
        /// </summary>
        public string FileTypeColumnName { get; set; }

        /// <summary>
        /// The name of the output column that will contain the file name
        /// </summary>
        public string OutputFileColumnName { get; set; }

        /// <summary>
        /// Construct new Mage file content processor module
        /// </summary>
        public FileContentProcessor()
        {
            SourceDirectoryColumnName = "Directory";
            FileTypeColumnName = string.Empty;
            SourceFileColumnName = "File";
            OutputFileColumnName = "File";
            OutputFileName = string.Empty;

            OutputColumnList = string.Format("{0}|+|text, {1}", OutputFileColumnName, SourceDirectoryColumnName);
            GetOutputFileName = GetDefaultOutputFileName;
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
                var concatenateOutput = !string.IsNullOrEmpty(OutputFileName);
                var sourceDirectory = args.Fields[InputColumnPos[SourceDirectoryColumnName]];
                var sourceFile = args.Fields[InputColumnPos[SourceFileColumnName]];

                string fileType;
                var sourceIsDirectory = false;

                if (string.IsNullOrWhiteSpace(FileTypeColumnName))
                {
                    if (sourceDirectory.StartsWith(MYEMSL_PATH_FLAG))
                    {
                        fileType = "file";
                    }
                    else if (Directory.Exists(sourceDirectory))
                    {
                        if (File.Exists(Path.Combine(sourceDirectory, sourceFile)))
                        {
                            fileType = "file";
                        }
                        else
                        {
                            UpdateStatus(this, new MageStatusEventArgs("FAILED->Cannot process directories with this processing mode", 1));
                            ReportMageWarning("Cannot process directories with this processing mode");
                            System.Threading.Thread.Sleep(500);
                            return;
                        }
                    }
                    else
                    {
                        UpdateStatus(this, new MageStatusEventArgs("FAILED->Directory Not Found: " + sourceDirectory, 1));
                        ReportMageWarning("Copy failed->Directory Not Found: " + sourceDirectory);
                        System.Threading.Thread.Sleep(250);
                        return;
                    }
                }
                else
                {
                    fileType = args.Fields[InputColumnPos[FileTypeColumnName]].ToLower();
                }

                if (fileType is "directory" or "folder")
                    sourceIsDirectory = true;

                string sourcePath;

                if (sourceIsDirectory)
                    sourcePath = sourceDirectory;
                else
                    sourcePath = Path.GetFullPath(Path.Combine(sourceDirectory, sourceFile));

                var destDirectory = OutputDirectoryPath;
                string destFile;

                if (sourceFile == kNoFilesFound)
                {
                    destFile = kNoFilesFound;
                }
                else
                {
                    if (concatenateOutput && !sourceIsDirectory)
                        destFile = OutputFileName;
                    else
                        destFile = GetOutputFileName(sourceFile, InputColumnPos, args.Fields);
                }

                var destPath = Path.GetFullPath(Path.Combine(destDirectory, destFile));

                // Package fields as dictionary
                var context = new Dictionary<string, string>();
                foreach (var colPos in InputColumnPos)
                {
                    context.Add(colPos.Key, args.Fields[colPos.Value] ?? string.Empty);
                }

                // Process file
                if (sourceFile == kNoFilesFound)
                {
                    // Skip
                }
                else if (string.IsNullOrEmpty(FileTypeColumnName) && fileType != "directory" && fileType != "folder")
                {
                    ProcessFile(sourceFile, sourcePath, destPath, context);
                }
                else
                {
                    if (fileType == "file")
                    {
                        ProcessFile(sourceFile, sourcePath, destPath, context);
                    }
                    if (fileType is "directory" or "folder")
                    {
                        ProcessDirectory(sourcePath, destPath);
                    }
                }

                var outRow = MapDataRow(args.Fields);

                var fileNameOutColIndex = OutputColumnPos[OutputFileColumnName];
                outRow[fileNameOutColIndex] = concatenateOutput ? sourceFile : destFile;

                // Strip off the MyEMSL FileID from the filename
                var myEMSLFileID = MyEMSLReader.DatasetInfoBase.ExtractMyEMSLFileID(outRow[fileNameOutColIndex], out var newFilePath);
                if (myEMSLFileID > 0)
                    outRow[fileNameOutColIndex] = newFilePath;

                OnDataRowAvailable(new MageDataEventArgs(outRow));
            }
            else
            {
                OnDataRowAvailable(new MageDataEventArgs(null));
            }
        }

        /// <summary>
        /// Handler for Mage standard tabular column definition
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            // Build lookup of column index by column name
            base.HandleColumnDef(sender, args);
            // End of column definitions from our source,

            // Now tell our subscribers what columns to expect from us
            ExportColumnDefs();
        }

        /// <summary>
        /// This function should be overridden by subclasses to do the actual processing
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="context"></param>
        protected virtual void ProcessFile(string sourceFile, string sourcePath, string destPath, Dictionary<string, string> context)
        {
        }

        /// <summary>
        /// This function should be overridden by subclasses to do the actual processing
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        protected virtual void ProcessDirectory(string sourcePath, string destPath)
        {
        }

        // Utility methods

        /// <summary>
        /// Default output file renamer
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="fieldPos"></param>
        /// <param name="fields"></param>
        protected string GetDefaultOutputFileName(string sourceFile, Dictionary<string, int> fieldPos, string[] fields)
        {
            return sourceFile;
        }

        // Methods for output columns

        // Tell our subscribers what columns to expect from us
        // which will be information about the files processed
        // plus any input columns that are passed through to output
        private void ExportColumnDefs()
        {
            OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
        }

        /// <summary>
        /// Update any interested listeners about our progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void UpdateStatus(object sender, MageStatusEventArgs args)
        {
            OnStatusMessageUpdated(args);
        }
    }
}
