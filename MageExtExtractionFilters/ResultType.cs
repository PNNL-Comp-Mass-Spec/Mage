using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MageExtContentFilters;
using System.Collections.ObjectModel;

namespace MageExtExtractionFilters {

    /// <summary>
    /// Describes key parameters for result files (and their associated files) 
    /// that extractor can handle
    /// </summary>
    public class ResultType {

        #region Constants
        
        public const string XTANDEM_ALL_PROTEINS = "X!Tandem All Proteins";
        public const string MSGFDB_SYN_ALL_PROTEINS = "MSGFDB Synopsis All Proteins";
		public const string INSPECT_SYN_ALL_PROTEINS = "Inspect Synopsis All Proteins";

        #endregion

        #region static properties and initializtion

        private static Dictionary<string, ResultType> mTypeList = new Dictionary<string, ResultType>();

        static ResultType() {
            foreach (ResultType rtype in Types) {
                mTypeList[rtype.ResultName] = rtype;
            }
        }

        public static Dictionary<string, ResultType> TypeList {
            get { return ResultType.mTypeList; }
        }

        private static List<ResultType> Types = new List<ResultType>() {
            //                 Name                         Tag        Filter     resultsFileTag      ResultsFileTag           IDColumnName
            new ResultType("Sequest Synopsis",              "syn",     "sequest",    "_syn.txt",         "_syn_MSGF.txt",         "HitNum"),
            new ResultType("Sequest First Hits",            "fht",     "sequest",    "_fht.txt",         "_fht_MSGF.txt",         "HitNum"),
            new ResultType("X!Tandem First Protein",        "xt",      "xtandem",    "_xt.txt",          "_xt_MSGF.txt",          "Result_ID"),
            new ResultType(XTANDEM_ALL_PROTEINS,            "xt",      "xtandem",    "_xt.txt",          "_xt_MSGF.txt",          "Result_ID"),
            new ResultType(INSPECT_SYN_ALL_PROTEINS,        "ins_syn", "inspect",    "_inspect_syn.txt", "_inspect_syn_MSGF.txt", "ResultID"),
            new ResultType("MSGFDB First Hits",             "msg_fht", "msgfdbFHT",  "_msgfdb_fht.txt",  "_msgfdb_fht_MSGF.txt",  "ResultID"),
            new ResultType("MSGFDB Synopsis First Protein", "msg_syn", "msgfdb",     "_msgfdb_syn.txt",  "_msgfdb_syn_MSGF.txt",  "ResultID"),
            new ResultType(MSGFDB_SYN_ALL_PROTEINS,         "msg_syn", "msgfdb",     "_msgfdb_syn.txt",  "_msgfdb_syn_MSGF.txt",  "ResultID") 
        };

        private static List<MergeFile> mMergeTypes = new List<MergeFile>() {

            //                 ResultName              NameColumn        KeyCol       FileNameTag
            { new MergeFile("X!Tandem First Protein", "ResultToSeqMap",  "Result_ID",     "_xt_ResultToSeqMap.txt") },
            { new MergeFile("X!Tandem First Protein", "SeqToProteinMap", "Unique_Seq_ID", "_xt_SeqToProteinMap.txt") },
            { new MergeFile("X!Tandem First Protein", "MSGF_Name",       "Result_ID",     "_xt_MSGF.txt") },

            { new MergeFile(XTANDEM_ALL_PROTEINS,     "ResultToSeqMap",  "Result_ID",     "_xt_ResultToSeqMap.txt") },
            { new MergeFile(XTANDEM_ALL_PROTEINS,     "SeqToProteinMap", "Unique_Seq_ID", "_xt_SeqToProteinMap.txt") },
            { new MergeFile(XTANDEM_ALL_PROTEINS,     "MSGF_Name",       "Result_ID",     "_xt_MSGF.txt") },

            { new MergeFile("Sequest Synopsis",       "MSGF_Name",       "Result_ID",     "_syn_MSGF.txt") },
            { new MergeFile("Sequest First Hits",     "MSGF_Name",       "Result_ID",     "_fht_MSGF.txt") },

            { new MergeFile(INSPECT_SYN_ALL_PROTEINS, "ResultToSeqMap",  "Result_ID",     "_inspect_syn_ResultToSeqMap.txt") },
            { new MergeFile(INSPECT_SYN_ALL_PROTEINS, "SeqToProteinMap", "Unique_Seq_ID", "_inspect_syn_SeqToProteinMap.txt") },
            { new MergeFile(INSPECT_SYN_ALL_PROTEINS, "MSGF_Name",       "Result_ID",     "_inspect_syn_MSGF.txt") },

            { new MergeFile("MSGFDB First Hits",      "MSGF_Name",       "Result_ID",     "_msgfdb_fht_MSGF.txt") },

            { new MergeFile("MSGFDB Synopsis First Protein", "ResultToSeqMap",  "Result_ID",     "_msgfdb_syn_ResultToSeqMap.txt") },
            { new MergeFile("MSGFDB Synopsis First Protein", "SeqToProteinMap", "Unique_Seq_ID", "_msgfdb_syn_SeqToProteinMap.txt") },
            { new MergeFile("MSGFDB Synopsis First Protein", "MSGF_Name",       "Result_ID",     "_msgfdb_syn_MSGF.txt") },

            { new MergeFile(MSGFDB_SYN_ALL_PROTEINS,         "ResultToSeqMap",  "Result_ID",     "_msgfdb_syn_ResultToSeqMap.txt") },
            { new MergeFile(MSGFDB_SYN_ALL_PROTEINS,         "SeqToProteinMap", "Unique_Seq_ID", "_msgfdb_syn_SeqToProteinMap.txt") },
            { new MergeFile(MSGFDB_SYN_ALL_PROTEINS,         "MSGF_Name",       "Result_ID",     "_msgfdb_syn_MSGF.txt") }

        };

        #endregion

        #region Properties

        public string ResultName { get; set; }
        public string Tag { get; set; }
        public string ResultsFileNamePattern { get; set; }
        public string Filter { get; set; }
        public string ResultIDColName { get; set; }
        public Collection<MergeFile> MergeFileTypes {
            get {
                Collection<MergeFile> types = new Collection<MergeFile>();
                foreach (MergeFile mf in mMergeTypes) {
                    if (mf.ResultName == ResultName) {
                        types.Add(mf);
                    }
                }
                return types;
            }
        }

        #endregion

        #region Internal Classes

        public class MergeFile {
            public string ResultName { get; set; }
            public string NameColumn { get; set; }
            public int ColumnIndx { get; set; }
            public string KeyCol { get; set; }
            public string FileNameTag { get; set; }
            public string MergeFileName { get; set; }

            public MergeFile(string resultName, string name, string keyCol, string tag) {
                ResultName = resultName;
                NameColumn = name;
                KeyCol = keyCol;
                FileNameTag = tag;
                ColumnIndx = -1;
                MergeFileName = "";
            }
        }

        #endregion

        #region Constructors

        public ResultType(string name, string tag, string filter, string resultsFileTag, string msgfFileTag, string idColName) {
            ResultName = name;
            Tag = tag;
            ResultsFileNamePattern = resultsFileTag;
            Filter = filter;
            ResultIDColName = idColName;
        }

        #endregion

        /// <summary>
        /// Return an extraction filter object for the current filter type
        /// </summary>
        /// <returns></returns>
        public ExtractionFilter GetExtractionFilter(FilterResultsBase resultsChecker) {
            ExtractionFilter exf = null;
            switch (Filter) {
                case "sequest":
                    SequestExtractionFilter sxf = new SequestExtractionFilter();
                    sxf.ResultChecker = resultsChecker as FilterSequestResults;
                    exf = sxf;
                    break;
                case "xtandem":
                    XTandemExtractionFilter xxf = new XTandemExtractionFilter();
                    xxf.ResultChecker = resultsChecker as FilterXTResults;
                    exf = xxf;
                    break;
                case "inspect":
					InspectExtractionFilter ixf = new InspectExtractionFilter();
					ixf.ResultChecker = resultsChecker as FilterInspectResults;
					exf = ixf;
                    break;
                case "msgfdbFHT":
                    MSGFDbFHTExtractionFilter mxf1 = new MSGFDbFHTExtractionFilter();
                    mxf1.ResultChecker = resultsChecker as FilterMSGFDbResults;
                    exf = mxf1;
                    break;

                case "msgfdb":
                    MSGFDbExtractionFilter mxf2 = new MSGFDbExtractionFilter();
                    mxf2.ResultChecker = resultsChecker as FilterMSGFDbResults;
                    exf = mxf2;
                    break;
                default:
                    exf = new ExtractionFilter();
                    break;
            }
            return exf;
        }

        /// <summary>
        /// Return a filter results checker object for the current filter type
        /// and given filter set
        /// </summary>
        /// <param name="filterSetID">Filter set ID</param>
        /// <returns></returns>
        public FilterResultsBase GetResultsChecker(string filterSetID) {
            FilterResultsBase frb = null;
            switch (Filter) {
                case "sequest":
                    frb = SequestExtractionFilter.MakeSequestResultChecker(filterSetID);
                    break;
                case "xtandem":
                    frb = XTandemExtractionFilter.MakeXTandemResultChecker(filterSetID);
                    break;
                case "inspect":
					frb = InspectExtractionFilter.MakeInspectResultChecker(filterSetID);
                    break;
                case "msgfdbFHT":
                    frb = MSGFDbFHTExtractionFilter.MakeMSGFDbResultChecker(filterSetID);
                    break;
                case "msgfdb":
                    frb = MSGFDbExtractionFilter.MakeMSGFDbResultChecker(filterSetID);
                    break;
                default:
                    break;
            }
            return frb;
        }
    }

}
