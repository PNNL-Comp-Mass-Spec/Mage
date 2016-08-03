
namespace MageExtExtractionFilters
{

    public class ExtractionType
    {

        public ResultType RType { get; set; }

        /// <summary>
        /// Retain results that don't pass results filter
        /// </summary>
        public string KeepAllResults { get; set; }

        /// <summary>
        /// Which result filter set (from DMS) to use
        /// </summary>
        public string ResultFilterSetID { get; set; }

        /// <summary>
        /// Eliminate results with MSGF SpecProb that don't meet this threshold
        /// This filter is used when merging search engine results with the _syn_MSGF.txt or _fht_MSGF.txt file
        /// </summary>
        public string MSGFCutoff { get; set; }

    }
}
