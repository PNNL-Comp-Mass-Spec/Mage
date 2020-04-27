using System.Collections.Generic;
using MageExtContentFilters;
using System.Collections.ObjectModel;

namespace MageExtExtractionFilters
{

    /// <summary>
    /// Describes key parameters for result files (and their associated files)
    /// that extractor can handle
    /// </summary>
    public class ResultType
    {

        #region Constants

        public const string XTANDEM_ALL_PROTEINS = "X!Tandem All Proteins";
        public const string MSGFDB_SYN_ALL_PROTEINS = "MSGF+ Synopsis All Proteins";
        public const string INSPECT_SYN_ALL_PROTEINS = "Inspect Synopsis All Proteins";
        public const string MSPATHFINDER_SYN_ALL_PROTEINS = "MSPathFinder All Proteins";

        #endregion

        #region static properties and initializtion

        static ResultType()
        {
            foreach (var rtype in Types)
            {
                TypeList[rtype.ResultName] = rtype;
            }
        }

        public static Dictionary<string, ResultType> TypeList { get; } = new Dictionary<string, ResultType>();

        private static readonly List<ResultType> Types = new List<ResultType>() {
            //                 Name                         Tag           Filter           resultsFileTag                       IDColumnName
            new ResultType("Sequest Synopsis",              "syn",        "sequest",       "_syn.txt",                          "HitNum"),
            new ResultType("Sequest First Hits",            "fht",        "sequest",       "_fht.txt",                          "HitNum"),
            new ResultType("X!Tandem First Protein",        "xt",         "xtandem",       "_xt.txt",                           "Result_ID"),
            new ResultType(XTANDEM_ALL_PROTEINS,            "xt",         "xtandem",       "_xt.txt",                           "Result_ID"),
            new ResultType(INSPECT_SYN_ALL_PROTEINS,        "ins_syn",    "inspect",       "_inspect_syn.txt",                  "ResultID"),
            new ResultType("MSGF+ First Hits",              "msg_fht",    "msgfplusFHT",   "_msgfplus_fht.txt;_msgfdb_fht.txt", "ResultID"),
            new ResultType("MSGF+ Synopsis First Protein",  "msg_syn",    "msgfplus",      "_msgfplus_syn.txt;_msgfdb_syn.txt", "ResultID"),
            new ResultType(MSGFDB_SYN_ALL_PROTEINS,         "msg_syn",    "msgfplus",      "_msgfplus_syn.txt;_msgfdb_syn.txt", "ResultID"),
            new ResultType("MSPathFinder First Protein",    "mspath_syn", "mspathfinder",  "_mspath_syn.txt",                   "ResultID"),
            new ResultType(MSPATHFINDER_SYN_ALL_PROTEINS,   "mspath_syn", "mspathfinder",  "_mspath_syn.txt",                   "ResultID")
        };

        private static readonly List<MergeFile> mMergeTypes = new List<MergeFile>() {
            //                 ResultName              NameColumn        KeyCol           FileNameTag
            new MergeFile("X!Tandem First Protein", "ResultToSeqMap",  "Result_ID",     "_xt_ResultToSeqMap.txt"),
            new MergeFile("X!Tandem First Protein", "SeqToProteinMap", "Unique_Seq_ID", "_xt_SeqToProteinMap.txt"),
            new MergeFile("X!Tandem First Protein", "MSGF_Name",       "Result_ID",     "_xt_MSGF.txt"),

            new MergeFile(XTANDEM_ALL_PROTEINS,     "ResultToSeqMap",  "Result_ID",     "_xt_ResultToSeqMap.txt"),
            new MergeFile(XTANDEM_ALL_PROTEINS,     "SeqToProteinMap", "Unique_Seq_ID", "_xt_SeqToProteinMap.txt"),
            new MergeFile(XTANDEM_ALL_PROTEINS,     "MSGF_Name",       "Result_ID",     "_xt_MSGF.txt"),

            new MergeFile("Sequest Synopsis",       "MSGF_Name",       "Result_ID",     "_syn_MSGF.txt"),
            new MergeFile("Sequest First Hits",     "MSGF_Name",       "Result_ID",     "_fht_MSGF.txt"),

            new MergeFile(INSPECT_SYN_ALL_PROTEINS, "ResultToSeqMap",  "Result_ID",     "_inspect_syn_ResultToSeqMap.txt"),
            new MergeFile(INSPECT_SYN_ALL_PROTEINS, "SeqToProteinMap", "Unique_Seq_ID", "_inspect_syn_SeqToProteinMap.txt"),
            new MergeFile(INSPECT_SYN_ALL_PROTEINS, "MSGF_Name",       "Result_ID",     "_inspect_syn_MSGF.txt"),

            // Note: for MSGF+ results, when instantiating the MergeFile instances we use the text msgfplus
            // That text will be auto-changed later on to msgfdb if necessary

            //                 ResultName                  NameColumn          KeyCol           FileNameTag
            new MergeFile("MSGF+ First Hits",              "MSGF_Name",       "Result_ID",     "_msgfplus_fht_MSGF.txt"),

            new MergeFile("MSGF+ Synopsis First Protein",  "ResultToSeqMap",  "Result_ID",     "_msgfplus_syn_ResultToSeqMap.txt"),
            new MergeFile("MSGF+ Synopsis First Protein",  "SeqToProteinMap", "Unique_Seq_ID", "_msgfplus_syn_SeqToProteinMap.txt"),
            new MergeFile("MSGF+ Synopsis First Protein",  "MSGF_Name",       "Result_ID",     "_msgfplus_syn_MSGF.txt"),

            new MergeFile(MSGFDB_SYN_ALL_PROTEINS,         "ResultToSeqMap",  "Result_ID",     "_msgfplus_syn_ResultToSeqMap.txt"),
            new MergeFile(MSGFDB_SYN_ALL_PROTEINS,         "SeqToProteinMap", "Unique_Seq_ID", "_msgfplus_syn_SeqToProteinMap.txt"),
            new MergeFile(MSGFDB_SYN_ALL_PROTEINS,         "MSGF_Name",       "Result_ID",     "_msgfplus_syn_MSGF.txt"),

            new MergeFile("MSPathFinder First Protein",    "ResultToSeqMap",  "Result_ID",     "_mspath_syn_ResultToSeqMap.txt"),
            new MergeFile("MSPathFinder First Protein",    "SeqToProteinMap", "Unique_Seq_ID", "_mspath_syn_SeqToProteinMap.txt"),

            new MergeFile(MSPATHFINDER_SYN_ALL_PROTEINS,   "ResultToSeqMap",  "Result_ID",     "_mspath_syn_ResultToSeqMap.txt"),
            new MergeFile(MSPATHFINDER_SYN_ALL_PROTEINS,   "SeqToProteinMap", "Unique_Seq_ID", "_mspath_syn_SeqToProteinMap.txt")

        };

        #endregion

        #region Properties

        /// <summary>
        /// User-friendly result name, for example "MSGF+ First Hits"
        /// </summary>
        public string ResultName { get; set; }

        /// <summary>
        /// File type tag
        /// </summary>
        /// <remarks>Valid values are syn, fht, msg_syn, msg_fht, etc.</remarks>
        public string Tag { get; set; }

        /// <summary>
        /// File name suffix to find
        /// </summary>
        /// <remarks>Separate a list of alternative suffixes using semicolons</remarks>
        public string ResultsFileNamePattern { get; set; }

        /// <summary>
        /// Search tool in use
        /// </summary>
        /// <remarks>Valid values are sequest, xtandem, msgfplus, msgfplusFHT, etc.</remarks>
        public string Filter { get; set; }

        /// <summary>
        /// Result ID column name
        /// </summary>
        public string ResultIDColName { get; set; }

        public Collection<MergeFile> MergeFileTypes
        {
            get
            {
                var types = new Collection<MergeFile>();
                foreach (var mf in mMergeTypes)
                {
                    if (mf.ResultName == ResultName)
                    {
                        types.Add(mf);
                    }
                }
                return types;
            }
        }

        #endregion

        #region Internal Classes

        public class MergeFile
        {
            public string ResultName { get; private set; }
            public string NameColumn { get; private set; }
            public int ColumnIndx { get; set; }
            public string KeyCol { get; private set; }
            public string FileNameTag { get; private set; }
            public string MergeFileName { get; set; }

            public MergeFile(string resultName, string name, string keyCol, string tag)
            {
                ResultName = resultName;
                NameColumn = name;
                KeyCol = keyCol;
                FileNameTag = tag;
                ColumnIndx = -1;
                MergeFileName = string.Empty;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="filter"></param>
        /// <param name="resultsFileTag">Filename suffix to match; separate a list of alternative suffixes using semicolons</param>
        /// <param name="idColName"></param>
        public ResultType(string name, string tag, string filter, string resultsFileTag, string idColName)
        {
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
        public ExtractionFilter GetExtractionFilter(FilterResultsBase resultsChecker)
        {
            ExtractionFilter exf;
            switch (Filter)
            {
                case "sequest":
                    var sxf = new SequestExtractionFilter { ResultChecker = resultsChecker as FilterSequestResults };
                    exf = sxf;
                    break;
                case "xtandem":
                    var xxf = new XTandemExtractionFilter { ResultChecker = resultsChecker as FilterXTResults };
                    exf = xxf;
                    break;
                case "inspect":
                    var ixf = new InspectExtractionFilter { ResultChecker = resultsChecker as FilterInspectResults };
                    exf = ixf;
                    break;
                case "msgfplusFHT":
                    var mxf1 = new MSGFDbFHTExtractionFilter { ResultChecker = resultsChecker as FilterMSGFDbResults };
                    exf = mxf1;
                    break;
                case "msgfplus":
                    var mxf2 = new MSGFDbExtractionFilter { ResultChecker = resultsChecker as FilterMSGFDbResults };
                    exf = mxf2;
                    break;
                case "mspathfinder":
                    var mspathxf = new MSPathFinderExtractionFilter { ResultChecker = resultsChecker as FilterMSPathFinderResults };
                    exf = mspathxf;
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
        public FilterResultsBase GetResultsChecker(string filterSetID)
        {
            FilterResultsBase frb = null;
            switch (Filter)
            {
                case "sequest":
                    frb = SequestExtractionFilter.MakeSequestResultChecker(filterSetID);
                    break;
                case "xtandem":
                    frb = XTandemExtractionFilter.MakeXTandemResultChecker(filterSetID);
                    break;
                case "inspect":
                    frb = InspectExtractionFilter.MakeInspectResultChecker(filterSetID);
                    break;
                case "msgfplusFHT":
                    frb = MSGFDbFHTExtractionFilter.MakeMSGFDbResultChecker(filterSetID);
                    break;
                case "msgfplus":
                    frb = MSGFDbExtractionFilter.MakeMSGFDbResultChecker(filterSetID);
                    break;
                case "mspathfinder":
                    frb = MSPathFinderExtractionFilter.MakeMSPathFinderResultChecker(filterSetID);
                    break;
                default:
                    break;
            }
            return frb;
        }
    }

}
