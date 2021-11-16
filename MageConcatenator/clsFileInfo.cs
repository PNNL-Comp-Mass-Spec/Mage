using System;
using System.IO;

namespace MageConcatenator
{
    internal class clsFileInfo
    {
        public string Name { get; set; }
        public string SizeKB { get; set; }
        public string DateModified { get; set; }
        public string DirectoryPath { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        public string FullPath
        {
            get
            {
                if (string.IsNullOrEmpty(DirectoryPath))
                    return Name;

                return Path.Combine(DirectoryPath, Name);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="filename"></param>
        public clsFileInfo(string directoryPath, string filename)
        {
            DirectoryPath = directoryPath;
            Name = filename;
        }
    }
}
