using System.Collections.Generic;
using System.Linq;
using Mage;

namespace BiodiversityFileCopy
{
  /// <summary>
  /// Extension of SinkWrapper that allows selective output
  /// according to limit on rows output 
  /// and/or simple whitelist filtering
  /// </summary>
  public class BoundedSinkWrapper : BaseModule
  {
    protected readonly SimpleSink WrappedSink;
    protected int MKeyColIdx;

    /// <summary>
    /// Maximum rows to output (defaults to all rows)
    /// </summary>
    public int NumRowsToOutput { get; set; }

    /// <summary>
    /// Name of column to apply whitelist filtering for 
    /// (Leave blank to disable filtering)
    /// </summary>
    public string KeyColName { get; set; }

    /// <summary>
    /// Whitelist of values to allow
    /// </summary>
    public List<string> KeyList { get; set; }

    /// <summary>
    /// constructor
    /// </summary>
    public BoundedSinkWrapper()
    {
    }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="sink"></param>
    public BoundedSinkWrapper(SimpleSink sink)
    {
      WrappedSink = sink;
      NumRowsToOutput = WrappedSink.Rows.Count;
    }

    public override void Prepare()
    {
      base.Prepare();
      MKeyColIdx = WrappedSink.ColumnIndex["Data_Package_ID"];
    }

    /// <summary>
    /// Serve contents of AddParentFolderToJobList object that we are wrapped around
    /// </summary>
    /// <param name="state"></param>
    public override void Run(object state)
    {
      OnColumnDefAvailable(new MageColumnEventArgs(WrappedSink.Columns.ToArray()));
      int outRowCount = 0;
      foreach (var row in WrappedSink.Rows) {
        if (Abort) break;
        if (outRowCount > NumRowsToOutput) break;
        if (KeyList == null || KeyList.Contains(row[MKeyColIdx])) {
          OnDataRowAvailable(new MageDataEventArgs(row));
          outRowCount++;
        }
      }
      OnDataRowAvailable(new MageDataEventArgs(null));
    }
  }

}
