using System.Collections.Generic;
using Mage;

namespace BiodiversityFileCopy
{
  /// <summary>
  /// Mage filter that adds organism column to output stream
  /// using lookup table
  /// </summary>
  public class AddOrganismNameFilter : ContentFilter
  {
    protected int PkgIdIdx;
    protected int OrgNameIdx;

    public string DataPackageIDColName { get; set; }
    public string OrgNameColName { get; set; }

    public Dictionary<string, string> OrganismLookup { get; set; }

    public override void HandleColumnDef(object sender, MageColumnEventArgs args)
    {
      base.HandleColumnDef(sender, args);
      PkgIdIdx = OutputColumnPos[DataPackageIDColName];
      OrgNameIdx = OutputColumnPos[OrgNameColName];
    }

    protected override bool CheckFilter(ref string[] vals)
    {
      if (OutputColumnDefs != null) {
        var outRow = MapDataRow(vals);

        var ogName = OrganismLookup[outRow[PkgIdIdx]];
        outRow[OrgNameIdx] = ogName;
        vals = outRow;
      }
      return true;
    }
  }
}
