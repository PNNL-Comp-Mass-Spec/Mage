using System.Collections.Generic;
using Mage;

namespace MageFileProcessor
{

    [MageAttribute("Filter", "XT2FHT", "Ascore XT2FHT", "Convert XT results files to SEQUEST FHT format")]
    class XT2FHTFilter : ContentFilter
    {

        // This is called for each row that is being subjected to filtering
        // the fields array contains value of each column for the row
        // the column index of each field can be looked up by field name in columnPos[]
        // to prevent the row from being sent to the ouput, return false
        protected override bool CheckFilter(ref string[] fields)
        {
            if (OutputColumnDefs != null)
            {
                fields = MapDataRow(fields);
            }
            return true;
        }

        /// <summary>
        /// Called before pipeline runs - module can do any special setup that it needs
        /// this filter module sets up its own column remapping
        /// (override of base class)
        /// </summary>
        public override void Prepare()
        {
            base.Prepare();
            OutputColumnList = GetFilterColumnMap();
        }

        // This is a function that returns an output column map
        private static string GetFilterColumnMap()
        {
            var colMapfields = new List<string>
            {
                "HitNum|+|text",
                "ScanNum|Scan",
                "ScanCount|+|text",
                "ChargeState|Charge",
                "MH|Peptide_MH",
                "XCorr|+|text",
                "DelCn|+|text",
                "Sp|+|text",
                "Reference|+|text",
                "MultiProtein|Multiple_Protein_Count",
                "Peptide|Peptide_Sequence",
                "DelCn2|DeltaCn2",
                "RankSp|+|text",
                "RankXc|+|text",
                "DelM|Delta_Mass",
                "XcRatio|+|text",
                "PassFilt|+|text",
                "MScore|+|text",
                "NumTrypticEnds|+|text"
            };
            return string.Join(", ", colMapfields);
        }

        /// <summary>
        /// Delegate that handles renaming of source file to output file
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="fieldPos"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public override string RenameOutputFile(string sourceFile, Dictionary<string, int> fieldPos, string[] fields)
        {
            return System.Text.RegularExpressions.Regex.Replace(sourceFile, "_xt", "_fht", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

    }
}
