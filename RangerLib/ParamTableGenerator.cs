﻿using System.Collections.Generic;
using Mage;

namespace RangerLib
{
    public class ParamTableGenerator
    {
        private readonly PermutationGenerator mPGenModule = new();

        /// <summary>
        /// Lookup for operator associated with a parameter name
        /// </summary>
        private readonly Dictionary<string, string> mParamOperatorLookup = new();

        /// <summary>
        /// Inform user of how many rows will be generated (after parameters are set)
        /// </summary>
        public int GeneratedParameterCount => mPGenModule.PredictedOutputRowCount;

        /// <summary>
        /// If output is delivered to file, set the path here if not, leave it blank
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// If output is delivered to SQLite DB, set path to DB here
        /// </summary>
        public string DBPath { get; set; }

        /// <summary>
        /// If output is delivered to SQLite DB, set table name here
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ParamTableGenerator()
        {
            FilePath = string.Empty;
            DBPath = string.Empty;
            TableName = string.Empty;
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

        public ProcessingPipeline GetPipeline()
        {
            // Build column lists and column overwrite specs
            var allCols = new List<string>();
            var operatorColOverwrites = new Dictionary<string, string>();

            foreach (var paramName in mPGenModule.ParamNames)
            {
                allCols.Add(paramName);
                var operatorColName = paramName + "_Operator";
                allCols.Add(operatorColName + "|+|text");
                operatorColOverwrites.Add(operatorColName, mParamOperatorLookup[paramName]);
            }
            var genColSpec = string.Join(", ", mPGenModule.ParamNames);
            var allColSpec = string.Join(", ", allCols.ToArray());

            // Set output column parameters for permutation generator module
            mPGenModule.AutoColumnName = "ref";
            mPGenModule.OutputColumnList = string.Format("{0}|+|text, {1}", mPGenModule.AutoColumnName, genColSpec);
            mPGenModule.AutoColumnFormat = "ParamSet_{0:000000}";

            // Create module to add columns for operators
            var filter = new NullFilter
            {
                OutputColumnList = mPGenModule.AutoColumnName + ", " + allColSpec
            };
            filter.SetContext(operatorColOverwrites);

            IBaseModule writer;
            if (!string.IsNullOrEmpty(FilePath))
            {
                // Create module to write to file
                var dfw = new DelimitedFileWriter
                {
                    FilePath = FilePath
                };
                writer = dfw;
            }
            else
            {
                // Create module to write to SQLite DB
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
