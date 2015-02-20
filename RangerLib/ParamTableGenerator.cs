using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;

namespace RangerLib
{

    public class ParamTableGenerator
    {

        private PermutationGenerator mPGenModule = new PermutationGenerator();

        /// <summary>
        /// Lookup for operator associated with a parameter name
        /// </summary>
        private Dictionary<string, string> mParamOperatorLookup = new Dictionary<string, string>();

        /// <summary>
        /// inform user of how many rows will be generated (after parameters are set)
        /// </summary>
        public int GeneratedParameterCount { get { return mPGenModule.PredictedOutputRowCount; } }

        /// <summary>
        ///  if ouput is delivered to file, set the path here if not, leave it blank
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// if output is delivered to SQLite DB, set path to DB here
        /// </summary>
        public string DBPath { get; set; }

        /// <summary>
        /// if output is delivered to SQLite DB, set table name here
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ParamTableGenerator()
        {
            FilePath = "";
            DBPath = "";
            TableName = "";
        }

        /// <summary>
        /// Add the specification for a parameter
        /// </summary>
        /// <param name="parms">
        /// "ParamName"
        /// "Lower"
        /// "Upper"
        /// "Increment"
        /// "IncrementList"
        /// "Operator"
        /// </param>
        public void AddParamColumn(Dictionary<string, string> parms)
        {
            mPGenModule.AddParamColumn(parms);
            mParamOperatorLookup.Add(parms["ParamName"], parms["Operator"]);
        }

        private void Clear()
        {
            mPGenModule = new PermutationGenerator();
            mParamOperatorLookup.Clear();
        }

        public ProcessingPipeline GetPipeline()
        {

            // build column lists and column overwrite specs
            var allCols = new List<string>();
            var operatorColOverwrites = new Dictionary<string, string>();

            foreach (string paramName in mPGenModule.ParamNames)
            {
                allCols.Add(paramName);
                string operatorColName = paramName + "_Operator";
                allCols.Add(operatorColName + "|+|text");
                operatorColOverwrites.Add(operatorColName, mParamOperatorLookup[paramName]);
            }
            string genColspec = string.Join(", ", mPGenModule.ParamNames);
            string allColspec = string.Join(", ", allCols.ToArray());

            // set output column parameters for permutation generator module
            mPGenModule.AutoColumnName = "ref";
            mPGenModule.OutputColumnList = string.Format("{0}|+|text, {1}", mPGenModule.AutoColumnName, genColspec);
            mPGenModule.AutoColumnFormat = "ParamSet_{0:000000}";

            // create module to add columns for operators
            var filter = new NullFilter
            {
                OutputColumnList = mPGenModule.AutoColumnName + ", " + allColspec
            };
            filter.SetContext(operatorColOverwrites);

            IBaseModule writer = null;
            if (!string.IsNullOrEmpty(FilePath))
            {
                // create module to write to file
                var dfw = new DelimitedFileWriter
                {
                    FilePath = FilePath
                };
                writer = dfw;
            }
            else
            {
                // create module to write to SQLite DB
                var slw = new SQLiteWriter
                {
                    DbPath = DBPath,
                    TableName = TableName
                };
                writer = slw;
            }
            return ProcessingPipeline.Assemble("ParamGen", mPGenModule, filter, writer);
        }

    }
}
