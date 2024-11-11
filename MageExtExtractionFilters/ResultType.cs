using System.Collections.Generic;
using MageExtContentFilters;
using System.Collections.ObjectModel;
using System;

namespace MageExtExtractionFilters
{
    /// <summary>
    /// Describes key parameters for result files (and their associated files)
    /// that extractor can handle
    /// </summary>
    public class ResultType
    {
        // Ignore Spelling: fht, Mage, MSFragger, msgfplus, msgfdb, mspathfinder, sequest, xt, xtandem

        public const string XTANDEM_ALL_PROTEINS = "X!Tandem All Proteins";
        public const string MSGFDB_SYN_ALL_PROTEINS = "MSGF+ Synopsis All Proteins";
        public const string INSPECT_SYN_ALL_PROTEINS = "Inspect Synopsis All Proteins";
        public const string MSPATHFINDER_SYN_ALL_PROTEINS = "MSPathFinder All Proteins";
        public const string MSFRAGGER_SYN_ALL_PROTEINS = "MSFragger All Proteins";
        public const string DIANN_SYN_ALL_PROTEINS = "DIA-NN All Proteins";

        static ResultType()
        {
            foreach (var resultType in Types)
            {
                TypeList[resultType.ResultName] = resultType;
            }
        }

        public static Dictionary<string, ResultType> TypeList { get; } = new();

        private static readonly List<ResultType> Types = new() {
            //                 Name                        Tag              Filter           resultsFileTag                       IDColumnName
            new ResultType("Sequest Synopsis",             "syn",           "sequest",       "_syn.txt",                          "HitNum"),
            new ResultType("Sequest First Hits",           "fht",           "sequest",       "_fht.txt",                          "HitNum"),
            new ResultType("X!Tandem First Protein",       "xt",            "xtandem",       "_xt.txt",                           "Result_ID"),
            new ResultType(XTANDEM_ALL_PROTEINS,           "xt",            "xtandem",       "_xt.txt",                           "Result_ID"),
            new ResultType(INSPECT_SYN_ALL_PROTEINS,       "ins_syn",       "inspect",       "_inspect_syn.txt",                  "ResultID"),
            new ResultType("MSGF+ First Hits",             "msg_fht",       "msgfplusFHT",   "_msgfplus_fht.txt;_msgfdb_fht.txt", "ResultID"),
            new ResultType("MSGF+ Synopsis First Protein", "msg_syn",       "msgfplus",      "_msgfplus_syn.txt;_msgfdb_syn.txt", "ResultID"),
            new ResultType(MSGFDB_SYN_ALL_PROTEINS,        "msg_syn",       "msgfplus",      "_msgfplus_syn.txt;_msgfdb_syn.txt", "ResultID"),
            new ResultType("MSPathFinder First Protein",   "mspath_syn",    "mspathfinder",  "_mspath_syn.txt",                   "ResultID"),
            new ResultType(MSPATHFINDER_SYN_ALL_PROTEINS,  "mspath_syn",    "mspathfinder",  "_mspath_syn.txt",                   "ResultID"),
            new ResultType(MSPATHFINDER_SYN_ALL_PROTEINS,  "mspath_syn",    "mspathfinder",  "_mspath_syn.txt",                   "ResultID"),
            new ResultType("MSFragger First Protein",      "msfragger_syn", "msfragger",     "_msfragger_syn.txt",                "ResultID"),
            new ResultType(MSFRAGGER_SYN_ALL_PROTEINS,     "msfragger_syn", "msfragger",     "_msfragger_syn.txt",                "ResultID"),
            new ResultType(MSFRAGGER_SYN_ALL_PROTEINS,     "msfragger_syn", "msfragger",     "_msfragger_syn.txt",                "ResultID"),
            new ResultType("DIA-NN First Protein",         "diann_syn",     "diann",         "_diann_syn.txt",                    "ResultID"),
            new ResultType(DIANN_SYN_ALL_PROTEINS,         "diann_syn",     "diann",         "_diann_syn.txt",                    "ResultID"),
            new ResultType(DIANN_SYN_ALL_PROTEINS,         "diann_syn",     "diann",         "_diann_syn.txt",                    "ResultID")
        };

        private static readonly List<MergeFile> mMergeTypes = new() {
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

            //                 ResultName                 NameColumn          KeyCol           FileNameTag
            new MergeFile("MSGF+ First Hits",             "MSGF_Name",       "Result_ID",     "_msgfplus_fht_MSGF.txt"),

            new MergeFile("MSGF+ Synopsis First Protein", "ResultToSeqMap",  "Result_ID",     "_msgfplus_syn_ResultToSeqMap.txt"),
            new MergeFile("MSGF+ Synopsis First Protein", "SeqToProteinMap", "Unique_Seq_ID", "_msgfplus_syn_SeqToProteinMap.txt"),
            new MergeFile("MSGF+ Synopsis First Protein", "MSGF_Name",       "Result_ID",     "_msgfplus_syn_MSGF.txt"),

            new MergeFile(MSGFDB_SYN_ALL_PROTEINS,        "ResultToSeqMap",  "Result_ID",     "_msgfplus_syn_ResultToSeqMap.txt"),
            new MergeFile(MSGFDB_SYN_ALL_PROTEINS,        "SeqToProteinMap", "Unique_Seq_ID", "_msgfplus_syn_SeqToProteinMap.txt"),
            new MergeFile(MSGFDB_SYN_ALL_PROTEINS,        "MSGF_Name",       "Result_ID",     "_msgfplus_syn_MSGF.txt"),

            new MergeFile("MSPathFinder First Protein",   "ResultToSeqMap",  "Result_ID",     "_mspath_syn_ResultToSeqMap.txt"),
            new MergeFile("MSPathFinder First Protein",   "SeqToProteinMap", "Unique_Seq_ID", "_mspath_syn_SeqToProteinMap.txt"),

            new MergeFile(MSPATHFINDER_SYN_ALL_PROTEINS,  "ResultToSeqMap",  "Result_ID",     "_mspath_syn_ResultToSeqMap.txt"),
            new MergeFile(MSPATHFINDER_SYN_ALL_PROTEINS,  "SeqToProteinMap", "Unique_Seq_ID", "_mspath_syn_SeqToProteinMap.txt"),

            new MergeFile("MSFragger First Protein",      "ResultToSeqMap",  "Result_ID",     "_msfragger_syn_ResultToSeqMap.txt"),
            new MergeFile("MSFragger First Protein",      "SeqToProteinMap", "Unique_Seq_ID", "_msfragger_syn_SeqToProteinMap.txt"),

            new MergeFile(MSFRAGGER_SYN_ALL_PROTEINS,     "ResultToSeqMap",  "Result_ID",     "_msfragger_syn_ResultToSeqMap.txt"),
            new MergeFile(MSFRAGGER_SYN_ALL_PROTEINS,     "SeqToProteinMap", "Unique_Seq_ID", "_msfragger_syn_SeqToProteinMap.txt"),

            new MergeFile("DIA-NN First Protein",         "ResultToSeqMap",  "Result_ID",     "_diann_syn_ResultToSeqMap.txt"),
            new MergeFile("DIA-NN First Protein",         "SeqToProteinMap", "Unique_Seq_ID", "_diann_syn_SeqToProteinMap.txt"),

            new MergeFile(DIANN_SYN_ALL_PROTEINS,         "ResultToSeqMap",  "Result_ID",     "_diann_syn_ResultToSeqMap.txt"),
            new MergeFile(DIANN_SYN_ALL_PROTEINS,         "SeqToProteinMap", "Unique_Seq_ID", "_diann_syn_SeqToProteinMap.txt")
        };

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

        public class MergeFile
        {
            public string ResultName { get; }
            public string NameColumn { get; }
            public int ColumnIndex { get; set; }
            public string KeyCol { get; }
            public string FileNameTag { get; }
            public string MergeFileName { get; set; }

            // ReSharper disable once ConvertToPrimaryConstructor
            public MergeFile(string resultName, string name, string keyCol, string tag)
            {
                ResultName = resultName;
                NameColumn = name;
                KeyCol = keyCol;
                FileNameTag = tag;
                ColumnIndex = -1;
                MergeFileName = string.Empty;
            }
        }

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

        /// <summary>
        /// Return an extraction filter object for the current filter type
        /// </summary>
        [Obsolete("Deprecated in 2024")]
        public ExtractionFilter GetExtractionFilter(FilterResultsBase resultsChecker)
        {
            return Filter switch
            {
                "sequest" => new SequestExtractionFilter { ResultChecker = resultsChecker as FilterSequestResults },
                "xtandem" => new XTandemExtractionFilter { ResultChecker = resultsChecker as FilterXTResults },
                "inspect" => new InspectExtractionFilter { ResultChecker = resultsChecker as FilterInspectResults },
                "msgfplusFHT" => new MSGFDbFHTExtractionFilter { ResultChecker = resultsChecker as FilterMSGFDbResults },
                "msgfplus" => new MSGFDbExtractionFilter { ResultChecker = resultsChecker as FilterMSGFDbResults },
                "mspathfinder" => new MSPathFinderExtractionFilter { ResultChecker = resultsChecker as FilterMSPathFinderResults },
                "msfragger" => new MSFraggerExtractionFilter { ResultChecker = resultsChecker as FilterMSFraggerResults },
                "diann" => new DiannExtractionFilter { ResultChecker = resultsChecker as FilterDiannResults },
                _ => new ExtractionFilter(),
            };
        }

        /// <summary>
        /// Return a filter results checker object for the current filter type
        /// and given filter set
        /// </summary>
        /// <param name="filterSetID">Filter set ID</param>
        [Obsolete("Deprecated in 2024")]
        public FilterResultsBase GetResultsChecker(string filterSetID)
        {
            return Filter switch
            {
                "sequest" => SequestExtractionFilter.MakeSequestResultChecker(filterSetID),
                "xtandem" => XTandemExtractionFilter.MakeXTandemResultChecker(filterSetID),
                "inspect" => InspectExtractionFilter.MakeInspectResultChecker(filterSetID),
                "msgfplusFHT" => MSGFDbFHTExtractionFilter.MakeMSGFDbResultChecker(filterSetID),
                "msgfplus" => MSGFDbExtractionFilter.MakeMSGFDbResultChecker(filterSetID),
                "mspathfinder" => MSPathFinderExtractionFilter.MakeMSPathFinderResultChecker(filterSetID),
                "msfragger" => MSFraggerExtractionFilter.MakeMSFraggerResultChecker(filterSetID),
                "diann" => DiannExtractionFilter.MakeDiannResultChecker(filterSetID),
                _ => null
            };
        }
    }
}
