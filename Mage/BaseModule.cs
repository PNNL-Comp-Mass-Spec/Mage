using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace Mage
{

    /// <summary>
    /// this class provides basic functions that are of use to most pipeline module classes.
    /// </summary>
    public class BaseModule : IBaseModule
    {

        private static readonly ILog traceLog = LogManager.GetLogger("TraceLog");

        /// <summary>
        /// Constant used when no files are found
        /// </summary>
        // ReSharper disable once MemberCanBeProtected.Global (used by the Analysis Manager)
        public const string kNoFilesFound = "--No Files Found--";

        #region Member Variables

        private int mNameDisambiguatorCount = 1;

        /// <summary>
        /// flag that is set by client (via pipeline infrastructure call to Cancel) to abort operation of module
        /// </summary>
        protected bool Abort
        {
            get { return Globals.AbortRequested; }
            set
            {
                if (value)
                    Globals.AbortRequested = true;
                else
                    Globals.AbortRequested = false;
            }
        }

        /// <summary>
        /// master list of input column definitions
        /// (default HandleColumnDef will build this) 
        /// </summary>
        protected readonly List<MageColumnDef> InputColumnDefs = new List<MageColumnDef>();

        /// <summary>
        /// master list of input column position keyed to column name (for lookup of column index by column name)
        /// (default HandleColumnDef will build this)
        /// </summary>
        protected Dictionary<string, int> InputColumnPos = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// temporary working reference to input column field used during output column mapping
        /// </summary>
        protected int InputColumnIndex;

        /// <summary>
        /// master list of Output column definitions
        /// (not all modules require this feature) 
        /// </summary>
        protected List<MageColumnDef> OutputColumnDefs;

        /// <summary>
        /// master list of Output column position keyed to column name (for lookup of column index by column name)
        /// (not all modules require this feature) 
        /// </summary>
        protected Dictionary<string, int> OutputColumnPos;

        /// <summary> 
        /// master list of position map between output columns and input columns
        /// </summary>
        protected List<KeyValuePair<int, int>> OutputToInputColumnPosMap;

        /// <summary>
        /// Set of key/value pairs for ad hoc parameters.
        /// </summary>
        protected Dictionary<string, string> Context;

        /// <summary>
        /// Position map list of new output columns 
        /// the have matching keys in the Context parameters
        /// </summary>
        protected Dictionary<string, int> ContextColPos;

        #endregion

        #region Properties

        /// <summary>
        /// comma-delimited list of specs for output columns that the module will supply to standard tabular output
        /// (this is only needed if module does not simply pass through the input columns)
        /// Col Specs:
        /// [output column name] - simple pass-through of input column with same name
        /// [output column name]|[input column name]|[type]|[size] - map input column to output column using different name and optionally override data type and size
        /// [output column name]|+|[type]|[size] - output column is new column
        /// </summary>
        /// <example>*, MSGF_SpecProb|+|text</example>
        /// <example>Dataset, Dataset_ID, Alias|+|text, *</example>
        /// <example>Item|+|text, Name|+|text, File_Size_KB|+|text, Folder, *</example>
        /// <example>ref|+|text, one, two, three, results|+|float</example>
        public string OutputColumnList { get; set; }

        #endregion

        /// <summary>
        /// Set the context parameters for the module (Optional)
        /// </summary>
        /// <param name="context">Set of parameters</param>
        public virtual void SetContext(Dictionary<string, string> context)
        {

            if (Context == null)
                Context = context;
            else
            {
                var mergedContext = new Dictionary<string, string>();

                // Update existing values
                foreach (var setting in Context)
                {
                    string newValue;
                    if (context.TryGetValue(setting.Key, out newValue))
                        mergedContext.Add(setting.Key, newValue);
                    else
                        mergedContext.Add(setting.Key, setting.Value);
                }

                // Add new values
                foreach (var setting in context)
                {
                    if (!Context.ContainsKey(setting.Key))
                        mergedContext.Add(setting.Key, setting.Value);
                }

                Context = mergedContext;
            }
        }

        #region IBaseModule Members

        /// <summary>
        /// event that is fired to send row data out via the module's standard tabular output
        /// </summary>
        public event EventHandler<MageDataEventArgs> DataRowAvailable;

        /// <summary>
        /// event that is fired to send column definitions out via the module's standard tabular output
        /// </summary>
        public event EventHandler<MageColumnEventArgs> ColumnDefAvailable;

        /// <summary>
        /// event that is fired to send a status update message
        /// </summary>
        public event EventHandler<MageStatusEventArgs> StatusMessageUpdated;

        /// <summary>
        /// event that is fired to send a warning message
        /// </summary>
        public event EventHandler<MageStatusEventArgs> WarningMessageUpdated;

        /// <summary>
        /// The event-invoking method that derived classes can override.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataRowAvailable(MageDataEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition
            var handler = DataRowAvailable;
            if (handler != null && !Abort)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The event-invoking method that derived classes can override.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnColumnDefAvailable(MageColumnEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition
            var handler = ColumnDefAvailable;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Event-invoking method for status message updates
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStatusMessageUpdated(MageStatusEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition
            var handler = StatusMessageUpdated;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Event-invoking method for warning messages
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnWarningMessage(MageStatusEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of a race condition
            var handler = WarningMessageUpdated;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Arbritrary name for a particular module object.
        /// This will be the same name that the pipeline has for this object.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// pipeline that contains this module (if any)
        /// </summary>
        public ProcessingPipeline Pipeline { get; set; }

        /// <summary>
        /// Called by pipeline container prior to pipeline execution beginning.
        /// Modules that need to do setup prior to receiving column definition
        /// and row data events on their standard tabular inputs should override this function.
        /// </summary>
        public virtual void Prepare()
        {
            InputColumnIndex = 0;
            Abort = false;
        }

        /// <summary>
        /// Called by pipeline container after pipeline execution terminates
        /// (even for error terminations)
        /// Modules that need to do clean up resources should override this function. 
        /// </summary>
        public virtual void Cleanup()
        {
            DataRowAvailable = null;
            ColumnDefAvailable = null;
            StatusMessageUpdated = null;
            WarningMessageUpdated = null;
        }

        /// <summary>
        /// Allows a string-valued property of this class (and its descendents)
        /// to be set by name
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void SetPropertyByName(string key, string val)
        {
            var pi = GetType().GetProperty(key);
            pi?.SetValue(this, val, null);
        }

        /// <summary>
        /// this implements the canonical mechanism for setting module parameters
        /// first, parameters are captured in a master key/value list
        /// next, the parameter list is traversed and any properties 
        /// whose name matches a parameter's key are set with the parameter's value
        /// </summary>
        /// <param name="parameters">List of key/value pairs for parameters (duplicate keys allowed)</param>
        public virtual void SetParameters(Dictionary<string, string> parameters)
        {
            if (parameters != null)
            {
                // set properties (of subclasses) from parameters
                foreach (var paramDef in parameters)
                {
                    SetPropertyByName(paramDef.Key, paramDef.Value);
                }
            }
        }

        /// <summary>
        /// handler for Mage standard tablular input data rows
        /// (override of base class)
        /// 
        /// This event handler receives row events from upstream module, one event per row.  
        /// on the module's standard tabular input, one event per row
        /// and a null vals object signalling the end of row events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void HandleDataRow(object sender, MageDataEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// handler for Mage standard tablular column definition
        /// (override of base class)
        /// 
        /// This event handler receives a column definition event
        /// on the module's standard tabular input.
        /// Subclasses should override this for any specialized column def handling
        /// that they need, but should be sure to also call the this base class function. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void HandleColumnDef(object sender, MageColumnEventArgs args)
        {
            foreach (var columnDef in args.ColumnDefs)
            {
                try
                {
                    // rename column if it has same name as previously handled column
                    if (InputColumnPos.ContainsKey(columnDef.Name))
                    {
                        columnDef.Name = columnDef.Name + (++mNameDisambiguatorCount);
                    }
                    InputColumnPos.Add(columnDef.Name, InputColumnIndex++);
                    InputColumnDefs.Add(columnDef);
                }
                catch (Exception e)
                {
                    traceLog.Error("HandleColumnDef:" + e.Message);
                    throw new MageException("HandleColumnDef:" + e.Message);
                }
            }
            SetupOutputColumns();
            SetupOutputColumnToContextMapping();
        }

        /// <summary>
        /// Modules that can be root modules must override this
        /// </summary>
        /// <param name="state">Provided so that this function has necessary signature 
        /// to be target of ThreadPool.QueueUserWorkItem</param>
        public virtual void Run(object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Terminate execution of this module
        /// </summary>
        public virtual void Cancel()
        {
            Abort = true;
        }

        /// <summary>
        /// Terminate the execution of the containing pipeline
        /// (if one exists)
        /// </summary>
        protected virtual void CancelPipeline()
        {
            // terminate the pipeline when row count is reached
            Pipeline?.Cancel();
        }

        /// <summary>
        /// Called by pipeline container after to pipeline execution has processed all of the data rows
        /// </summary>
        public virtual bool PostProcess()
        {
            return true;
        }

        /// <summary>
        /// Raise a status message event that processing was aborted
        /// </summary>
        protected void ReportProcessingAborted()
        {
            ReportProcessingAborted(ModuleName);
        }

        /// <summary>
        /// Raise a status message event that processing was aborted
        /// </summary>
        /// <param name="source">The source procedure name</param>
        protected void ReportProcessingAborted(string source)
        {
            if (string.IsNullOrEmpty(source))
                OnStatusMessageUpdated(new MageStatusEventArgs("Processing aborted"));
            else
                OnStatusMessageUpdated(new MageStatusEventArgs(source + ": Processing aborted"));
        }

        #endregion


        #region helper functions

        /// <summary>
        /// Returns the index of columnName in columnPos
        /// </summary>
        /// <param name="columnPos">Dictionary of column position information</param>
        /// <param name="columnName">Column to find</param>
        /// <returns>Index if defined; otherwise, returns -1</returns>
        protected static int GetColumnIndex(Dictionary<string, int> columnPos, string columnName)
        {
            int value;

            if (columnPos.TryGetValue(columnName, out value))
                return value;
            else
                return -1;
        }

        /// <summary>
        /// Returns the value at the given index in the columnVals array
        /// </summary>
        /// <param name="columnVals">Column Data</param>
        /// <param name="columnIndex">Index of the column to return</param>
        /// <param name="defaultValue">Value to return if columnIndex is less than 0 or if the entry is not numeric</param>
        /// <returns>Value (integer) if defined; otherwise, returns defaultValue</returns>
        protected int GetColumnValue(string[] columnVals, int columnIndex, int defaultValue)
        {
            if (columnIndex > -1)
            {
                int value;
                if (columnVals[columnIndex] != null && int.TryParse(columnVals[columnIndex], out value))
                    return value;
                else
                    return defaultValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the value at the given index in the columnVals array
        /// </summary>
        /// <param name="columnVals">Column Data</param>
        /// <param name="columnIndex">Index of the column to return</param>
        /// <param name="defaultValue">Value to return if columnIndex is less than 0 or if the entry is not numeric</param>
        /// <returns>Value (double) if defined; otherwise, returns defaultValue</returns>
        protected double GetColumnValue(string[] columnVals, int columnIndex, double defaultValue)
        {
            if (columnIndex > -1)
            {
                double value;
                if (columnVals[columnIndex] != null && double.TryParse(columnVals[columnIndex], out value))
                    return value;
                else
                    return defaultValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the value at the given index in the columnVals array
        /// </summary>
        /// <param name="columnVals">Column Data</param>
        /// <param name="columnIndex">Index of the column to return</param>
        /// <param name="defaultValue">Value to return if columnIndex is less than 0</param>
        /// <returns>Value (string) if defined; otherwise, returns defaultValue</returns>
        protected string GetColumnValue(string[] columnVals, int columnIndex, string defaultValue)
        {
            if (columnIndex > -1)
            {
                if (columnVals[columnIndex] != null)
                    return columnVals[columnIndex];
                else
                    return string.Empty;
            }
            return defaultValue;
        }

        /// <summary>
        /// Return True if optionValue is Yes or True or 1
        /// </summary>
        /// <param name="optionValue"></param>
        /// <returns></returns>
        protected bool OptionEnabled(string optionValue)
        {
            return string.Equals(optionValue, "Yes", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(optionValue, "True", StringComparison.InvariantCultureIgnoreCase) ||
                   string.Equals(optionValue, "1", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Perform case insenstive replacement string replace
        /// </summary>
        /// <param name="original">Text to search</param>
        /// <param name="pattern">Text to find</param>
        /// <param name="replacement">Replacement text</param>
        /// <returns>The updated string</returns>
        /// <remarks>From "http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx" using Michael Epner's </remarks>
        protected string ReplaceEx(string original, string pattern, string replacement)
        {
            return ReplaceEx(original, pattern, replacement, StringComparison.OrdinalIgnoreCase, -1);
        }

        /// <summary>
        /// Perform case insenstive replacement string replace
        /// </summary>
        /// <param name="original">Text to search</param>
        /// <param name="pattern">Text to find</param>
        /// <param name="replacement">Replacement text</param>
        /// <param name="comparisonType">Comparison type; use StringComparison.OrdinalIgnoreCase for case-insensitive</param>
        /// <returns>The updated string</returns>
        /// <remarks>From "http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx" using Michael Epner's </remarks>
        public string ReplaceEx(string original, string pattern, string replacement, StringComparison comparisonType)
        {
            return ReplaceEx(original, pattern, replacement, comparisonType, -1);
        }

        /// <summary>
        /// Perform case insenstive replacement string replace
        /// </summary>
        /// <param name="original">Text to search</param>
        /// <param name="pattern">Text to find</param>
        /// <param name="replacement">Replacement text</param>
        /// <param name="comparisonType">Comparison type; use StringComparison.OrdinalIgnoreCase for case-insensitive</param>
        /// <param name="stringBuilderInitialSize">Initial size of the string builder</param>
        /// <returns>The updated string</returns>
        /// <remarks>From "http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx" using Michael Epner's </remarks>
        public string ReplaceEx(string original, string pattern, string replacement, StringComparison comparisonType, int stringBuilderInitialSize)
        {
            if (original == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(pattern))
            {
                return original;
            }


            var posCurrent = 0;
            var lenPattern = pattern.Length;
            var idxNext = original.IndexOf(pattern, comparisonType);
            var result = new StringBuilder(stringBuilderInitialSize < 0 ? Math.Min(4096, original.Length) : stringBuilderInitialSize);

            while (idxNext >= 0)
            {
                result.Append(original, posCurrent, idxNext - posCurrent);
                result.Append(replacement);

                posCurrent = idxNext + lenPattern;

                idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
            }

            result.Append(original, posCurrent, original.Length - posCurrent);

            return result.ToString();
        }


        /// <summary>
        /// Setup position map list of new output columns 
        /// the have matching keys in the Context parameters
        /// </summary>
        protected void SetupOutputColumnToContextMapping()
        {
            if (Context != null && NewOutputColumnPos != null)
            {
                ContextColPos = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
                foreach (var colPos in NewOutputColumnPos)
                {
                    if (Context.ContainsKey(colPos.Key))
                    {
                        ContextColPos.Add(colPos.Key, colPos.Value);
                    }
                }
            }
        }

        /// <summary>
        /// if there are any columns defined in the OutputColumnList property
        /// populate the appropriate internal buffers with column definitions
        /// and field indexes for them
        /// </summary>
        /// <remarks>See the comments for OutputColumnList for example output column definitions</remarks>
        protected void SetupOutputColumns()
        {
            if (string.IsNullOrEmpty(OutputColumnList))
                return;

            OutputColumnPos = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            OutputColumnDefs = new List<MageColumnDef>();
            OutputToInputColumnPosMap = new List<KeyValuePair<int, int>>();
            NewOutputColumnPos = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

            var outColIdx = 0;
            // process each column spec from spec list
            try
            {
                if (OutputColumnList.StartsWith("Job, "))
                {
                    if (!InputColumnPos.ContainsKey("Job"))
                    {
                        // Job number is not available; don't try to add it
                        OutputColumnList = OutputColumnList.Substring(5);
                    }
                }

                foreach (var colSpec in OutputColumnList.Split(','))
                {
                    // break each column spec into fields
                    var colSpecFlds = colSpec.Trim().Split('|');
                    var outputColName = colSpecFlds[0].Trim();
                    var inputColName = (colSpecFlds.Length > 1) ? colSpecFlds[1].Trim() : "";
                    var type = (colSpecFlds.Length > 2) ? colSpecFlds[2].Trim() : "";
                    var size = (colSpecFlds.Length > 3) ? colSpecFlds[3].Trim() : "";

                    if (outputColName == "*")
                    {
                        // wildcard
                        // copy input column def to output col def for any input columns
                        // not already in output colums list
                        outColIdx = MapOutputColumnsForUnmappedInputColumns(outColIdx);
                        continue;
                    }
                    if (inputColName == "+")
                    {
                        // output column is new column not found in input
                        size = (!string.IsNullOrEmpty(size)) ? size : "10";
                        AddOutputColumnDefinition(new MageColumnDef(outputColName, type, size), outColIdx);
                        AddIndexForNewColumn(outputColName, outColIdx);
                        outColIdx++;
                        continue;
                    }

                    // output column is mapped to input column
                    // copy input column def to output col def for this column
                    var colName = (string.IsNullOrEmpty(inputColName)) ? outputColName : inputColName;
                    MapOutputColumnToInputColumn(colName, outColIdx);
                    // and do any necessary overrides
                    AdjustOutputColumnProperties(outColIdx, outputColName, type, size);
                    outColIdx++;
                }
            }
            catch (Exception e)
            {
                traceLog.Error(e.Message);
                throw new MageException("Problem with defining output columns:" + e.Message);
            }
        }

        /// <summary>
        /// A name/position map for "new" output columns (columns added to output that don't remap input columns)
        /// (not all modules require this feature) 
        /// </summary>
        protected Dictionary<string, int> NewOutputColumnPos;

        private void AddIndexForNewColumn(string outputColName, int outColIdx)
        {
            NewOutputColumnPos.Add(outputColName, outColIdx);
        }

        /// <summary>
        /// Add output column definitions that are a pass-through of an input column
        /// for any input columns not already mapped to output colums list                       
        /// </summary>
        private int MapOutputColumnsForUnmappedInputColumns(int outColIdx)
        {
            foreach (var inputColDef in InputColumnDefs)
            {
                var inputColName = inputColDef.Name;
                if (!OutputColumnPos.ContainsKey(inputColName))
                {
                    MapOutputColumnToInputColumn(inputColName, outColIdx);
                    outColIdx++;
                }
            }
            return outColIdx;
        }

        /// <summary>
        /// Change one or more of the output column properties of the column definition 
        /// at the index position
        /// </summary>
        /// <param name="outColIdx">Index position of column definition to change</param>
        /// <param name="name">New name (or ignore if blank)</param>
        /// <param name="type">New data type (or ignore it and size if blank)</param>
        /// <param name="size">New data size</param>
        private void AdjustOutputColumnProperties(int outColIdx, string name, string type, string size)
        {
            if (outColIdx < 0 && outColIdx >= OutputColumnDefs.Count)
                return;

            if (!string.IsNullOrEmpty(name))
            {
                OutputColumnDefs[outColIdx].Name = name;
            }
            if (!string.IsNullOrEmpty(type))
            {
                OutputColumnDefs[outColIdx].DataType = type;
                OutputColumnDefs[outColIdx].Size = size;
            }
        }

        /// <summary>
        /// Add output column definition that is pass-through of an input column
        /// </summary>
        private void MapOutputColumnToInputColumn(string inputColName, int outColIdx)
        {
            if (!InputColumnPos.ContainsKey(inputColName))
            {
                throw new Exception($"Tried to map input column '{inputColName}' which does not exist");
            }
            var inputColIdx = InputColumnPos[inputColName];
            var colDef = InputColumnDefs[inputColIdx];
            AddOutputColumnDefinition(colDef, outColIdx, inputColIdx);
        }

        /// <summary>
        /// Add column definition to output column definition list and index lookup, 
        /// and to output-to-input column map
        /// </summary>
        private void AddOutputColumnDefinition(MageColumnDef colDef, int outColIdx, int inputColIdx)
        {
            AddOutputColumnDefinition(new MageColumnDef(colDef.Name, colDef.DataType, colDef.Size), outColIdx);
            OutputToInputColumnPosMap.Add(new KeyValuePair<int, int>(outColIdx, inputColIdx));
        }

        /// <summary>
        /// Add column definition to output column definition list and index lookup, 
        /// </summary>
        private void AddOutputColumnDefinition(MageColumnDef colDef, int outColIdx)
        {
            OutputColumnDefs.Add(colDef);
            OutputColumnPos.Add(colDef.Name, outColIdx);
        }

        /// <summary>
        /// if the module is using output column definition for output rows
        /// (instead of defaulting to using the input column definition)
        /// this function will create an output row according to output column
        /// definition
        /// </summary>
        /// <param name="vals">An input data row with fields according to input column definitions</param>
        /// <returns></returns>
        protected string[] MapDataRow(string[] vals)
        {
            // remap results according to our output column definitions
            var outRow = new string[OutputColumnDefs.Count];

            var actualCount = vals.Length;

            // copy over values from remapped input columns
            foreach (var colMap in OutputToInputColumnPosMap)
            {
                if (colMap.Value < actualCount)
                {
                    outRow[colMap.Key] = vals[colMap.Value];
                }
                else
                {
                    outRow[colMap.Key] = "";
                }
            }

            // add any matching context values to new columns
            if (ContextColPos != null)
            {
                foreach (var newCol in ContextColPos)
                {
                    outRow[newCol.Value] = Context[newCol.Key];
                }
            }
            return outRow;
        }

        /// <summary>
        /// Returns the column index defined in OutputColumnPos for column columnName
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="colIndex"></param>
        /// <returns>Index if defined; -1 if columnName is not present in OutputColumnPos</returns>
        protected bool TryGetOutputColumnPos(string columnName, out int colIndex)
        {

            if (OutputColumnPos != null)
            {
                if (OutputColumnPos.TryGetValue(columnName, out colIndex))
                    return true;
            }

            colIndex = -1;
            return false;
        }

        #endregion

    }
}
