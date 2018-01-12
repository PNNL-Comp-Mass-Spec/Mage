using Mage;

namespace BiodiversityFileCopy
{
  /// <summary>
  /// Base class for Mage filter to add source and destination
  /// file path columns to output stream
  /// </summary>
  public abstract class BaseFilePathsFilter : ContentFilter
  {
    protected int SourceFldrIdx;
    protected int DatasetIdx;
    protected int PkgIdIdx;
    protected int SourceFileIdx;
    protected int DestFileIdx;
    protected int OrgNameIdx;

    public string DataPackageIDColName { get; set; }
    public string DatasetColName { get; set; }
    public string OrganismNameColumn { get; set; }

    public string SourceFolderPathColName { get; set; }
    public string DestinationRootFolderPath { get; set; }
    public string SourceFilePathColName { get; set; }
    public string DestinationFilePathColName { get; set; }

    public string OutputSubfolderName { get; set; }

    public override void HandleColumnDef(object sender, MageColumnEventArgs args)
    {
      base.HandleColumnDef(sender, args);
      SourceFldrIdx = OutputColumnPos[SourceFolderPathColName];
      DatasetIdx = OutputColumnPos[DatasetColName];
      PkgIdIdx = OutputColumnPos[DataPackageIDColName];
      SourceFileIdx = OutputColumnPos[SourceFilePathColName];
      DestFileIdx = OutputColumnPos[DestinationFilePathColName];
      OrgNameIdx = OutputColumnPos[OrganismNameColumn];
    }

    protected override bool CheckFilter(ref string[] vals)
    {
      if (OutputColumnDefs != null) {
        var outRow = MapDataRow(vals);
        var destFilepath = "";
        var srcFilePath = "";

        var go = BuildPaths(outRow, ref srcFilePath, ref destFilepath);
        if (go) {
          outRow[SourceFileIdx] = srcFilePath;
          outRow[DestFileIdx] = destFilepath;
          vals = outRow;
        }
        return go;
      }
      return false;
    }

    /// <summary>
    /// This function is overridden by subclasses
    /// to build the actual source and destination file paths
    /// </summary>
    /// <param name="outRow">current row to be output into the stream</param>
    /// <param name="srcFilePath">full path to source file</param>
    /// <param name="destFilepath">full path to destination file</param>
    /// <returns>should row actually be added to output stream</returns>
    public abstract bool BuildPaths(string[] outRow, ref string srcFilePath, ref string destFilepath);

    public virtual void SetDefaultProperties(string outputRootFolderPath, string outputSubfolderName)
    {
      OutputSubfolderName = outputSubfolderName;
      OutputColumnList = "SourceFilePath|+|text, DestinationFilePath|+|text, *";
      SourceFilePathColName = "SourceFilePath";
      DestinationFilePathColName = "DestinationFilePath";
      DestinationRootFolderPath = outputRootFolderPath;
      DataPackageIDColName = "Data_Package_ID";
      OrganismNameColumn = "OG_Name";
      SourceFolderPathColName = "Folder";
      DatasetColName = "Dataset";
    }
  }

}
