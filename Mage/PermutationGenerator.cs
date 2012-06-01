using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using System.Collections.ObjectModel;

namespace Mage {

    /// <summary>
    /// Generates all possible permutations of a set of parameters
    /// as standard tabular output
    /// </summary>
    public class PermutationGenerator : BaseModule {

        #region Member Variables

        /// <summary>
        /// list of definitions for the parameters that will be permutated
        /// and included in the standard tabular output as columns 
        /// </summary>
        private List<ParameterDef> mParamColDefinitions = new List<ParameterDef>();

        private int mTotalRows = 1;

        private int mAutoColumnIndex = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Include header row in generated data
        /// </summary>
        public bool IncludeHeaderInOutput { get; set; }

        /// <summary>
        /// number of active parameters
        /// </summary>
        public int ParamCount { get { return mParamColDefinitions.Count; } }

        /// <summary>
        /// number of permutation rows that will be generated
        /// </summary>
        public int PredictedOutputRowCount { get { return mTotalRows; } }

        /// <summary>
        /// name of identifier column
        /// </summary>
        public string AutoColumnName { get; set; }

        /// <summary>
        /// format of identifier column
        /// </summary>
        public string AutoColumnFormat { get; set; }

        /// <summary>
        /// initial sequence number for identifier column
        /// </summary>
        public int AutoColumnSeed { get; set; }

        /// <summary>
        /// list of active parameters names
        /// </summary>
        public Collection<string> ParamNames {
            get {
                Collection<string> pn = new Collection<string>();
                foreach (ParameterDef pd in mParamColDefinitions) {
                    pn.Add(pd.ParamName);
                }
                return pn;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// construct a new empty Mage permutation generator object
        /// </summary>
        public PermutationGenerator() {
            IncludeHeaderInOutput = true;
            AutoColumnName = "";
            AutoColumnFormat = "ParamSet{0:000000}";
            AutoColumnSeed = 1;
        }

        #endregion

        #region Client Accessible Functions

        /// <summary>
        /// Add a parameter column definition.  The parameter will be incremented by the step 
        /// ammount withing the range set by the lower and upper bounds.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="lower">Lower bound of the parameter</param>
        /// <param name="upper">Upper bound of the parameter</param>
        /// <param name="step">Amount to increment by</param>
        public void AddParamColumn(string name, string lower, string upper, string step) {
            ParameterDef pDef = new ParameterDef(name, lower, upper, step);
            mParamColDefinitions.Add(pDef);
            SetCycleCountsForParamterColDefList();
        }

        /// <summary>
        /// Add a parameter column definition.  The parameter will be
        /// incremented according to parameters supplied as key/value pairs 
        /// </summary>
        /// <param name="parms">parameters as key/value pairs</param>
        public void AddParamColumn(Dictionary<string, string> parms) {
            ParameterDef pDef = new ParameterDef(parms);
            mParamColDefinitions.Add(pDef);
            SetCycleCountsForParamterColDefList();
        }

        #endregion

        #region IBaseModule Members

        /// <summary>
        /// generate data and output it
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state) {
            SetupInputColumns();
            SetupAutoColumn();
            if (IncludeHeaderInOutput) {
                OutputHeaderLine();
            }
            GenerateRows();
        }

        #endregion

        #region Private Functions

        // set up auto column
        private void SetupAutoColumn() {
            mAutoColumnIndex = -1;
            if (!string.IsNullOrEmpty(AutoColumnName)) {
                if (OutputColumnPos.ContainsKey(AutoColumnName)) {
                    mAutoColumnIndex = OutputColumnPos[AutoColumnName];
                }
            }
        }

        // set up to use BaseModule internal column handling
        private void SetupInputColumns() {
            List<MageColumnDef> colDefs = new List<MageColumnDef>();
            foreach (ParameterDef pDef in mParamColDefinitions) {
                MageColumnDef colDef = new MageColumnDef(pDef.ParamName, "float", "10");
                colDefs.Add(colDef);
            }
            base.HandleColumnDef(this, new MageColumnEventArgs(colDefs.ToArray()));
        }

        // set the row cycle count for each parameter def object 
        // and get total row count that will be produced
        private void SetCycleCountsForParamterColDefList() {
            // set cycle count for each parameter def object
            // and get total row count that will be produced
            mTotalRows = 1;
            foreach (ParameterDef pDef in mParamColDefinitions) {
                pDef.RowCycle = mTotalRows;
                mTotalRows *= pDef.NumberOfIncrements;
            }
        }

        // generate output rows given parameter column definitions
        // and total row count and output them via standard tabular output
        private void GenerateRows() {
            int totalCols = mParamColDefinitions.Count;
            // step through all row numbers for output rows
            // and generate a row for each and add it to list
            for (int rowNum = 0; rowNum < mTotalRows; rowNum++) {
                if (Abort) {
                    ReportProcessingAborted();
                    break;
                }
                // make new empty row
                string[] row = new string[totalCols];
                // step through each column and update row fields
                // using previously set up column parameter objects
                for (int colNum = 0; colNum < totalCols; colNum++) {
                    ParameterDef pDef = mParamColDefinitions[colNum];
                    row[colNum] = pDef.CurrentIncrement(rowNum).ToString();
                }
                OutputDataLine(row, rowNum);
            }
            OutputDataLine(null, 0);
        }

        // send the data row information to any listeners
        // via standard tabular output
        private void OutputDataLine(string[] fields, int rowNum) {
            if (fields == null) {
                OnDataRowAvailable(new MageDataEventArgs(fields));
                return;
            }
            if (OutputColumnDefs == null) {
                OnDataRowAvailable(new MageDataEventArgs(fields));
            } else {
                object[] outRow = MapDataRow(fields);
                for (int i = 0; i < outRow.Length; i++) {
                    if (outRow[i] == null) {
                        outRow[i] = "";
                    }
                }
                if (mAutoColumnIndex > -1) {
                    outRow[mAutoColumnIndex] = string.Format(AutoColumnFormat, rowNum + AutoColumnSeed);
                }
                OnDataRowAvailable(new MageDataEventArgs(outRow));
            }
        }

        // send the header information to any listeners
        // via standard tabular output
        private void OutputHeaderLine() {
            // output the column definitions
            if (OutputColumnDefs != null) {
                OnColumnDefAvailable(new MageColumnEventArgs(OutputColumnDefs.ToArray()));
            } else {
                OnColumnDefAvailable(new MageColumnEventArgs(InputColumnDefs.ToArray()));
            }
        }

        #endregion

        #region Private classes

        // Class that provides permutation behavior 
        // for a single parameter
        private class ParameterDef {
            public string ParamName { get; set; }

            // increment range parameters
            // used to calculate specific incremental values
            public double UpperBound { get; set; }
            public double LowerBound { get; set; }
            public double Step { get; set; }

            // list (comma-delimited) of explicit
            // incremental values
            public string IncrementList { get; set; }

            // list of increment values that this parameter object
            // will cycle through.  Increment values are either calculated 
            // from increment range parameters or supplied as an explicit list
            private List<string> increments = new List<string>();
            public int NumberOfIncrements { get { return increments.Count; } }

            // number of output rows that must pass
            // before this object's parameter increment values
            // advances to the next value.
            public int RowCycle { get; set; }

            // index to current increment
            private int incrementIndex = 0;

            // get the current parameter increment value for 
            // parameter represented by this object, based on
            // the rowNum and cycle (which must be set externally)
            public string CurrentIncrement(int rowNum) {
                if ((rowNum != 0) && (rowNum % RowCycle == 0)) {
                    incrementIndex = ++incrementIndex % increments.Count;
                }
                return increments[incrementIndex];
            }

            private void Initialize() {
                ParamName = "";
                UpperBound = 0;
                LowerBound = 0;
                Step = 0;
                RowCycle = 1;
                IncrementList = "";
            }

            // constructor
            public ParameterDef() {
            }

            // constructor
            public ParameterDef(Dictionary<string, string> paramList) {
                Initialize();
                double value = 0;
                foreach (KeyValuePair<string, string> paramDef in paramList) {
                    switch (paramDef.Key) {
                        case "ParamName":
                            ParamName = paramDef.Value;
                            break;
                        case "Lower":
                            value = 0;
                            if (double.TryParse(paramDef.Value, out value)) {
                                LowerBound = value;
                            }
                            break;
                        case "Upper":
                            value = 0;
                            if (double.TryParse(paramDef.Value, out value)) {
                                UpperBound = value;
                            }
                            break;
                        case "Increment":
                            value = 0;
                            if (double.TryParse(paramDef.Value, out value)) {
                                Step = value;
                            }
                            break;
                        case "IncrementList":
                            IncrementList = paramDef.Value;
                            break;
                        default:
                            break;
                    }
                }
                CalculateIncrements();
            }


            // constructor
            public ParameterDef(string name, string lower, string upper, string step) {
                Initialize();
                ParamName = name;
                double value = 0;

                value = 0;
                if (double.TryParse(lower, out value)) {
                    LowerBound = value;
                }
                value = 0;
                if (double.TryParse(upper, out value)) {
                    UpperBound = value;
                }
                value = 0;
                if (double.TryParse(step, out value)) {
                    Step = value;
                }
                CalculateIncrements();
            }


            // set up list of increment values 
            // based on increment range parameters
            // or list of increment values
            private void CalculateIncrements() {
                if (!string.IsNullOrEmpty(IncrementList) && Step == 0) {
                    increments.AddRange(IncrementList.Split(','));
                } else {
                    double current = LowerBound;
                    for (int j = 0; (current <= UpperBound); j++) {
                        current = LowerBound + j * Step;
                        if (current > UpperBound) break;
                        increments.Add(current.ToString());
                    }
                }
            }

        }

        #endregion
    }

}
