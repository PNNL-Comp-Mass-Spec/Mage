using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Mage;

namespace MageExtExtractionFilters
{

    /// <summary>
    /// Provides information about where processed results are to be delivered
    /// and 
    /// </summary>
    public class DestinationType
    {

        public enum Types { Unknown, File_Output, SQLite_Output }

        #region Properties

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
                var destName = "";
                switch (Type)
                {
                    case DestinationType.Types.SQLite_Output:
                        destName = string.Format("metadata_{0:yyyy-MM-dd_hhmmss}", System.DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Name + "_metadata";
                        }
                        break;
                    case DestinationType.Types.File_Output:
                        destName = string.Format("metadata_{0:yyyy-MM-dd_hhmmss}.txt", System.DateTime.Now);
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
                var destName = "";
                switch (Type)
                {
                    case DestinationType.Types.SQLite_Output:
                        destName = string.Format("filter_criteria_{0:yyyy-MM-dd_hhmmss}", System.DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Name + "_filter_criteria";
                        }
                        break;
                    case DestinationType.Types.File_Output:
                        destName = string.Format("filter_criteria_{0:yyyy-MM-dd_hhmmss}.txt", System.DateTime.Now);
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
                var destName = "";
                switch (Type)
                {
                    case DestinationType.Types.SQLite_Output:
                        destName = string.Format("file_list_{0:yyyy-MM-dd_hhmmss}", System.DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Name + "_file_list";
                        }
                        break;
                    case DestinationType.Types.File_Output:
                        destName = string.Format("file_list_{0:yyyy-MM-dd_hhmmss}.txt", System.DateTime.Now);
                        if (!string.IsNullOrEmpty(Name))
                        {
                            destName = Path.GetFileNameWithoutExtension(Name) + "_file_list.txt";
                        }
                        break;
                }
                return destName;
            }
        }

        #endregion

        #region Constructors

        public DestinationType(string type, string path, string name)
        {
            ContainerPath = path;
            Name = name;
            switch (type)
            {
                case "File_Output":
                    Type = Types.File_Output;
                    break;
                case "SQLite_Output":
                    Type = Types.SQLite_Output;
                    break;
                default:
                    Type = Types.Unknown;
                    break;
            }
        }

        #endregion


        /// <summary>
        /// If destination is file, return its full path
        /// otherwise return blank
        /// </summary>
        public string FilePath
        {
            get
            {
                var filePath = "";
                if (Type == Types.File_Output && !string.IsNullOrEmpty(Name))
                {
                    filePath = Path.Combine(ContainerPath, Name);
                }
                return filePath;
            }
        }

        #region Utiltity Functions

        /// <summary>
        /// If the output is set to a concatentated output file, and it exists,
        /// give user options.
        /// </summary>
        /// <returns></returns>
        public static bool VerifyDestinationOptionsWithUser(DestinationType destination)
        {
            var ok = true;
            if (!string.IsNullOrEmpty(destination.FilePath))
            {
                var sb = new StringBuilder();
                sb.AppendLine(string.Format("A copy of file '{0}' exists.  What action do you wish to take?", destination.Name));
                sb.AppendLine("");
                sb.AppendLine("Yes - delete existing file and continue with extraction");
                sb.AppendLine("No - retain existing file and append extracted results to it");
                sb.AppendLine("Cancel - retain existing file and abort extraction");
                if (File.Exists(destination.FilePath))
                {
                    var dr = MessageBox.Show(sb.ToString(), "Output file options", MessageBoxButtons.YesNoCancel);
                    switch (dr)
                    {
                        case DialogResult.Cancel:
                            ok = false;
                            break;
                        case DialogResult.No:
                            break;
                        case DialogResult.Yes:
                            try
                            {
                                File.Delete(destination.FilePath);
                            }
                            catch (Exception e)
                            {
                                if (e is IOException)
                                {
                                    MessageBox.Show(e.Message);
                                    ok = false;
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            break;
                    }
                }
            }
            // TODO: check that dataset name is file and not folder
            return ok;
        }

        #endregion

        /// <summary>
        /// get destination writer module based on destination type
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public BaseModule GetDestinationWriterModule(string prefix, string inputFilePath)
        {
            BaseModule writer = null;
            string autoName;
            string destName;
            switch (this.Type)
            {
                case DestinationType.Types.SQLite_Output:
                    autoName = prefix + "_" + Path.GetFileNameWithoutExtension(inputFilePath);
                    destName = (!string.IsNullOrEmpty(this.Name)) ? this.Name : autoName;
                    var sw = new SQLiteWriter
                    {
                        DbPath = this.ContainerPath,
                        TableName = destName.Replace("-", "_")
                    };
                    writer = sw;
                    break;
                case DestinationType.Types.File_Output:
                    autoName = prefix + "_" + Path.GetFileName(inputFilePath);
                    destName = (!string.IsNullOrEmpty(this.Name)) ? this.Name : autoName;
                    var dw = new DelimitedFileWriter
                    {
                        FilePath = Path.Combine(this.ContainerPath, destName),
                        Append = "Yes"
                    };
                    // TODO: only append if concatenating
                    writer = dw;
                    break;
            }
            return writer;
        }


    }
}
