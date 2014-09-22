
namespace MageExtExtractionFilters {

    public class ExtractionType {

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
        /// </summary>
        public string MSGFCutoff { get; set; }


    }
}
