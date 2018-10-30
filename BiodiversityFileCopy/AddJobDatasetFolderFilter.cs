using System.IO;
using Mage;

namespace BiodiversityFileCopy
{
    /// <summary>
    /// Mage filter to add dataset directory column to stream
    /// based on job results directory path
    /// </summary>
    public class AddJobDatasetDirectoryFilter : ContentFilter
    {
        protected int DatasetDirectoryIndex;
        protected int JobResultsDirectoryIndex;

        public string DatasetDirectoryColName { get; set; }
        public string JobResultsDirectoryColName { get; set; }

        public override void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            base.HandleColumnDef(sender, args);
            DatasetDirectoryIndex = OutputColumnPos[DatasetDirectoryColName];
            JobResultsDirectoryIndex = OutputColumnPos[JobResultsDirectoryColName];
        }

        protected override bool CheckFilter(ref string[] vals)
        {
            if (OutputColumnDefs != null)
            {
                var outRow = MapDataRow(vals);

                var resultsDirectoryPath = outRow[JobResultsDirectoryIndex];
                outRow[DatasetDirectoryIndex] = Path.GetDirectoryName(resultsDirectoryPath);
                vals = outRow;
            }
            return true;
        }
    }

}
