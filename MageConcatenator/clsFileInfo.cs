using System.IO;

namespace MageConcatenator
{
    class clsFileInfo
    {
        public string Name { get; set; }
        public string SizeKB { get; set; }
        public string DateModified { get; set; }
        public string FolderPath { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        public string FullPath
        {
            get
            {
                if (string.IsNullOrEmpty(FolderPath))
                    return Name;

                return Path.Combine(FolderPath, Name);
            }
        }

        public clsFileInfo(string filename)
        {
            FolderPath = string.Empty;
            Name = filename;
        }


        public clsFileInfo(string folderPath, string filename)
        {
            FolderPath = folderPath;
            Name = filename;
        }

    }
}
