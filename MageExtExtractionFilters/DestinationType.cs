﻿using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Mage;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// Provides information about where processed results are to be delivered
    /// </summary>
    public class DestinationType
    {
        // Ignore Spelling: Mage

        public enum Types { Unknown, File_Output, SQLite_Output }

        public Types Type { get; }

        public string ContainerPath { get; }

        public string Name { get; }

        /// <summary>
        /// Return an appropriate name for the metadata file or table
        /// </summary>
        public string MetadataName
        {
            get
            {
                var destName = string.Empty;
                switch (Type)
                {
                    case Types.SQLite_Output:
                        destName = string.Format("metadata_{0:yyyy-MM-dd_hhmmss}", DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Name + "_metadata";
                        }
                        break;
                    case Types.File_Output:
                        destName = string.Format("metadata_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Path.GetFileNameWithoutExtension(Name) + "_metadata.txt";
                        }
                        break;
                }
                return destName;
            }
        }

        /// <summary>
        /// Return an appropriate name for the FilterCriteria file or table
        /// </summary>
        public string FilterCriteriaName
        {
            get
            {
                var destName = string.Empty;
                switch (Type)
                {
                    case Types.SQLite_Output:
                        destName = string.Format("filter_criteria_{0:yyyy-MM-dd_hhmmss}", DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Name + "_filter_criteria";
                        }
                        break;
                    case Types.File_Output:
                        destName = string.Format("filter_criteria_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Path.GetFileNameWithoutExtension(Name) + "_filter_criteria.txt";
                        }
                        break;
                }
                return destName;
            }
        }

        /// <summary>
        /// Return an appropriate name for the manifest file or table
        /// </summary>
        public string FileListName
        {
            get
            {
                var destName = string.Empty;
                switch (Type)
                {
                    case Types.SQLite_Output:
                        destName = string.Format("file_list_{0:yyyy-MM-dd_hhmmss}", DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Name + "_file_list";
                        }
                        break;
                    case Types.File_Output:
                        destName = string.Format("file_list_{0:yyyy-MM-dd_hhmmss}.txt", DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Path.GetFileNameWithoutExtension(Name) + "_file_list.txt";
                        }
                        break;
                }
                return destName;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="path">Path</param>
        /// <param name="name">Name</param>
        public DestinationType(string type, string path, string name)
        {
            ContainerPath = path;
            Name = name;
            Type = type switch
            {
                "File_Output" => Types.File_Output,
                "SQLite_Output" => Types.SQLite_Output,
                _ => Types.Unknown
            };
        }

        /// <summary>
        /// If destination is file, return its full path
        /// otherwise return blank
        /// </summary>
        public string FilePath
        {
            get
            {
                var filePath = string.Empty;
                if (Type == Types.File_Output && !string.IsNullOrEmpty(Name))
                {
                    filePath = Path.Combine(ContainerPath, Name);
                }
                return filePath;
            }
        }

        // Utility Methods

        /// <summary>
        /// If the output is set to a concatenated output file, and it exists, ask the user what to do
        /// </summary>
        public static bool VerifyDestinationOptionsWithUser(DestinationType destination)
        {
            if (string.IsNullOrEmpty(destination.FilePath))
                return true;

            var sb = new StringBuilder();
            sb.AppendFormat("A copy of file '{0}' exists.  What action do you wish to take?", destination.Name).AppendLine();
            sb.AppendLine();
            sb.AppendLine("Yes - delete existing file and continue with extraction");
            sb.AppendLine("No - retain existing file and append extracted results to it");
            sb.AppendLine("Cancel - retain existing file and abort extraction");

            if (!File.Exists(destination.FilePath))
                return true;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (MessageBox.Show(sb.ToString(), "Output file options", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Cancel:
                    return false;
                case DialogResult.No:
                    break;
                case DialogResult.Yes:
                    try
                    {
                        File.Delete(destination.FilePath);
                    }
                    catch (IOException e)
                    {
                        MessageBox.Show(e.Message);
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Get destination writer module based on destination type
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="inputFilePath"></param>
        public BaseModule GetDestinationWriterModule(string prefix, string inputFilePath)
        {
            string autoName;
            string destName;

            switch (Type)
            {
                case Types.SQLite_Output:
                    autoName = prefix + "_" + Path.GetFileNameWithoutExtension(inputFilePath);
                    destName = !string.IsNullOrEmpty(Name) ? Name : autoName;

                    return new SQLiteWriter
                    {
                        DbPath = ContainerPath,
                        TableName = destName.Replace("-", "_")
                    };

                case Types.File_Output:
                    autoName = prefix + "_" + Path.GetFileName(inputFilePath);
                    destName = !string.IsNullOrEmpty(Name) ? Name : autoName;

                    // TODO: only append if concatenating
                    return new DelimitedFileWriter
                    {
                        FilePath = Path.Combine(ContainerPath, destName),
                        Append = "Yes"
                    };
            }

            return null;
        }
    }
}
