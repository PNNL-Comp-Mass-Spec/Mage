using System.Collections.Generic;
using System.IO;
using Mage;

namespace BiodiversityFileCopy
{
    /// <summary>
    /// Mage filter to add dataset folder column to stream
    /// based on job results folder path
    /// </summary>
    public class AddJobDatasetFolderFilter : ContentFilter
    {
        protected int DatasetFolderIdx;
        protected int JobResultsFolderIndex;

        public string DatasetFolderColName { get; set; }
        public string JobResultsFolderColName { get; set; }

        public Dictionary<string, string> OrganismLookup { get; set; }

        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            DatasetFolderIdx = OutputColumnPos[DatasetFolderColName];
            JobResultsFolderIndex = OutputColumnPos[JobResultsFolderColName];
        }

        protected override bool CheckFilter(ref string[] vals)
        {
            if (OutputColumnDefs != null)
            {
                var outRow = MapDataRow(vals);

                var resultsFolderPath = outRow[JobResultsFolderIndex];
                outRow[DatasetFolderIdx] = Path.GetDirectoryName(resultsFolderPath);
                vals = outRow;
            }
            return true;
        }
    }

}
