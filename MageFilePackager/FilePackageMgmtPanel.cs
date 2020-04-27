using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageFilePackager
{

    public partial class FilePackageMgmtPanel : UserControl, IModuleParameters
    {

        // public event EventHandler<MageCommandEventArgs> OnAction;

        #region Member Variables

        // Path for local copy of manifest file
        private string _outputPath;

        #endregion

        public FilePackageMgmtPanel()
        {
            InitializeComponent();
        }

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>
                       {
                { "OutputFilePath",   OutputFilePath}
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "OutputFilePath":
                        OutputFilePath = paramDef.Value;
                        break;
                }
            }
        }

        #endregion

        #region Properties

        public string ListTitle
        {
            get => packageListDisplayControl1.PageTitle;
            set => packageListDisplayControl1.PageTitle = value;
        }

        // Display list that provides new content to import into file package
        public GridViewDisplayControl FileSourceList { get; set; }

        public string FileListLabelPrefix { get; set; }

        public string TotalSizeDisplay
        {
            get => ContentInfoCtl.Text;
            set => ContentInfoCtl.Text = value;
        }

        // Local path for manifest file
        public string OutputFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(_outputPath) && !Path.HasExtension(_outputPath))
                {
                    if (_outputPath.EndsWith("\\"))
                        _outputPath += "download_pkg";
                    _outputPath += ".xml";
                }
                return _outputPath;
            }
            set => _outputPath = value;
        }

        #endregion

        #region Package Content Actions

        private void ClearPackageList()
        {
            var result = MessageBox.Show("Are you sure?", "Delete current package contents", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                packageListDisplayControl1.Clear();
                TotalSizeDisplay = string.Empty;
            }
        }

        /// <summary>
        /// Add selected files from file list to package list
        /// </summary>
        private void AddSelectedFilesToPackageList()
        {
            var entityType = FileSourceList.PageTitle.Replace(FileListLabelPrefix, "");
            SetPackageContentUsingRemapping(entityType, new GVPipelineSource(FileSourceList, "selected"));
        }

        /// <summary>
        /// Add all files from file list to package list
        /// </summary>
        private void AddAllFilesToPackageList()
        {
            var entityType = FileSourceList.PageTitle.Replace(FileListLabelPrefix, "");
            SetPackageContentUsingRemapping(entityType, new GVPipelineSource(FileSourceList, "All"));
        }

        /// <summary>
        /// Add selected files from tree display of files to package list
        /// </summary>
        private void AddFilesToPackageListFromTree()
        {
            if (FileSourceList.List.RowCount == 0)
            {
                MessageBox.Show("File list is empty");
                return;
            }
            var entityType = FileSourceList.PageTitle.Replace(FileListLabelPrefix, "");
            var nfSource = new GVPipelineSource(FileSourceList, "All");
            var packageFilter = GetPackageFilter(entityType);

            var fileTree = new FileTreeForm { FileListSource = nfSource, PackageFilter = packageFilter };
            if (fileTree.ShowDialog() == DialogResult.OK)
            {
                var treeSource = fileTree.GetSource();
                SetPackageContentWithoutRemapping(treeSource);
            }
        }

        /// <summary>
        /// Set package contents display (don't remap columns)
        /// </summary>
        /// <param name="filesToAdd"></param>
        public void SetPackageContentWithoutRemapping(BaseModule filesToAdd)
        {
            var newContents = GetCurrentPackageContents();
            ProcessingPipeline.Assemble("Add", filesToAdd, newContents).RunRoot(null);
            SetPackageContents(newContents);
            TotalSizeDisplay = TotalGB(TotalKB());
        }

        /// <summary>
        /// Set package contents display (use column remapping)
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="fileToAdd"></param>
        public void SetPackageContentUsingRemapping(string entityType, BaseModule fileToAdd)
        {
            var newContents = GetCurrentPackageContents();
            var packageFilter = GetPackageFilter(entityType);
            ProcessingPipeline.Assemble("Add", fileToAdd, packageFilter, newContents).RunRoot(null);
            SetPackageContents(newContents);
            TotalSizeDisplay = TotalGB(TotalKB());
        }

        /// <summary>
        /// Replace contents of package list with contents of source
        /// </summary>
        /// <param name="newContents">Contains contents to set</param>
        private void SetPackageContents(SimpleSink newContents)
        {
            var sink = packageListDisplayControl1.MakeSink(ListTitle, 15);
            ProcessingPipeline.Assemble("PackageFileContent", new SinkWrapper(newContents), sink).RunRoot(null);
        }

        /// <summary>
        /// Get current package contents
        /// </summary>
        /// <returns></returns>
        private SimpleSink GetCurrentPackageContents()
        {
            var nvSink = new Accumulator();
            // Save current content of package from content display
            if (packageListDisplayControl1.ItemCount > 0)
            {
                var cvSource = new GVPipelineSource(packageListDisplayControl1, "All");
                ProcessingPipeline.Assemble("Remember", cvSource, nvSink).RunRoot(null);
                nvSink.RetainColumnDefs = true;
            }
            return nvSink;
        }

        #endregion

        #region File List Actions

        /// <summary>
        /// Display contents of file package in tree view
        /// and allow user to choose which files to keep
        /// </summary>
        private void EditPackageListAsTree()
        {
            if (packageListDisplayControl1.List.RowCount == 0)
            {
                MessageBox.Show("Package list is empty");
                return;
            }
            var fileTree = new FileTreeForm { FileListSource = new GVPipelineSource(packageListDisplayControl1, "All") };
            if (fileTree.ShowDialog() == DialogResult.OK)
            {
                var treeSource = fileTree.GetSource();
                SetPackageContentWithoutRemapping(treeSource);
            }
        }

        /// <summary>
        /// Save current contents of file package to CSV file
        /// </summary>
        private void SavePackageListToFile()
        {
            if (packageListDisplayControl1.ItemCount == 0)
            {
                MessageBox.Show("Package list is empty");
                return;
            }
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = "txt",
                FileName = _outputPath,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            OutputFilePath = saveFileDialog.FileName;
            try
            {
                IBaseModule source = new GVPipelineSource(packageListDisplayControl1, "All");
                var writer = new DelimitedFileWriter { FilePath = _outputPath };
                ProcessingPipeline.Assemble("SaveListDisplayPipeline", source, writer).RunRoot(null);
            }
            catch (MageException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Replace contents of file package from CSV file
        /// </summary>
        private void LoadPackageListFromFile()
        {
            var openFileDialog1 = new OpenFileDialog
            {
                RestoreDirectory = true,
                AddExtension = true,
                CheckFileExists = false,
                DefaultExt = "txt",
                Filter = "Text|*.txt;|All files (*.*)|*.*"
            };
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            OutputFilePath = openFileDialog1.FileName;
            try
            {
                var reader = new DelimitedFileReader { FilePath = _outputPath };
                ProcessingPipeline.Assemble("RestoreListDisplayPipeline", reader, packageListDisplayControl1).RunRoot(null);
                TotalSizeDisplay = TotalGB(TotalKB());
            }
            catch (MageException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion

        #region Make Manifiest Actions

        /// <summary>
        /// Create XML manifest for file package contents
        /// and optionally save to file and/or submit to DMS web service
        /// </summary>
        private void MakeManifest()
        {
            if (packageListDisplayControl1.ItemCount == 0)
            {
                MessageBox.Show("Package list is empty");
                return;
            }
            var submissionForm = new SubmissionForm();
            if (submissionForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            var parameters = new Dictionary<string, string>
                                 {
                                     {"email", submissionForm.NotificationEmail},
                                     {"Package_Name", submissionForm.PackageName},
                                     {"Package_Description", submissionForm.PackageDescription},
                                     {"Total_Size_KB", string.Format("{0}", TotalKB())}
                                 };

            BaseModule gvPipelineSource = new GVPipelineSource(packageListDisplayControl1, "All");
            var xmlSink = new XMLSink
            {
                Prefixes = FilePackageFilter.PrefixList,
                Parameters = parameters
            };
            ProcessingPipeline.Assemble("XML", gvPipelineSource, xmlSink).RunRoot(null);

            if (submissionForm.SaveToFile)
            {
                using (var writer = new StreamWriter(submissionForm.ManifestFilePath))
                {
                    writer.Write(xmlSink.Text);
                }
            }

            if (submissionForm.SendToServer)
            {
                var postDataList = new Dictionary<string, string>
                                       {
                                           {"manifest", xmlSink.Text}
                                       };

                try
                {
                    var responseData = DMSHttp.Post(submissionForm.URL, postDataList);
                    if (!string.IsNullOrWhiteSpace(responseData))
                        MessageBox.Show(responseData, "Response");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Submitting to DMS");
                }
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get total size of all files in file package
        /// </summary>
        /// <param name="totalKB"> </param>
        /// <returns></returns>
        private string TotalGB(float totalKB)
        {
            return string.Format("{0:###,###,##0.0 GBytes}", totalKB / 1048576);
        }

        /// <summary>
        /// Calculate total size of all files in file package
        /// </summary>
        /// <returns></returns>
        private float TotalKB()
        {
            float totalKB = 0;

            // Find file size column
            var colIndex = -1;
            foreach (DataGridViewColumn col in packageListDisplayControl1.List.Columns)
            {
                if (col.Name == "KB")
                    colIndex = col.Index;
            }

            // Build total
            if (colIndex >= 0)
            {
                foreach (DataGridViewRow row in packageListDisplayControl1.List.Rows)
                {
                    float.TryParse(row.Cells[colIndex].Value.ToString(), out var temp);
                    totalKB += temp;
                }
            }
            return totalKB;
        }

        /// <summary>
        /// Create Mage module that removes prefixes from file paths according to source entity type
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private FilePackageFilter GetPackageFilter(string entityType)
        {
            const string baseColMap = "Item, Name, KB|" + FileListInfoBase.COLUMN_NAME_FILE_SIZE + ", Path|+|text, Source|+|text, ";
            var idColMap = "ID";
            var sourceType = "unknown";
            switch (entityType)
            {
                case "Data Packages":
                    idColMap = " ID";
                    sourceType = "Data_Package";
                    break;
                case "Datasets":
                    idColMap = "ID|Dataset_ID";
                    sourceType = "Dataset";
                    break;
                case "Jobs":
                    idColMap = "ID|Job";
                    sourceType = "Job";
                    break;
            }
            var filter = new FilePackageFilter();
            filter.SetContext(new Dictionary<string, string> { { "Source", sourceType } });
            filter.OutputColumnList = baseColMap + idColMap;
            return filter;
        }

        #endregion

        #region Control Event Handlers

        private void SaveBtnClick(object sender, EventArgs e)
        {
            SavePackageListToFile();
        }
        private void OpenBtnClick(object sender, EventArgs e)
        {
            LoadPackageListFromFile();
        }
        private void AddFromTreeBtnClick(object sender, EventArgs e)
        {
            AddFilesToPackageListFromTree();
        }
        private void SubmitBtnClick(object sender, EventArgs e)
        {
            MakeManifest();
        }
        private void ClearBtnClick(object sender, EventArgs e)
        {
            ClearPackageList();
        }
        private void AddSelectedFilesBtnClick(object sender, EventArgs e)
        {
            AddSelectedFilesToPackageList();
        }
        private void AddAllFilesBtnClick(object sender, EventArgs e)
        {
            AddAllFilesToPackageList();
        }
        private void EditPackageListBtnClick(object sender, EventArgs e)
        {
            EditPackageListAsTree();
        }

        #endregion



    }
}
