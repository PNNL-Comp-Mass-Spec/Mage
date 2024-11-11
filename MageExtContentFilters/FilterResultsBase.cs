using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MageExtContentFilters
{
    public class FilterResultsBase
    {
        // Ignore Spelling: Mage, Regex

        protected static int CriteriaGroupIDIndex = 0;
        protected static int CriteriaNameIndex = 1;
        protected static int CriteriaOperatorIndex = 2;
        protected static int CriteriaValueIntIndex = 3;
        protected static int CriterionIDIndex = 4;

        // Internal class
        protected class FilterCriteriaDef
        {
            public string CriteriaName;
            public string CriteriaOperator;
            public int CriteriaValueInt;
            public float CriteriaValueFloat;
            public int CriterionID;

            public FilterCriteriaDef(IReadOnlyList<string> criteria)
            {
                CriteriaName = criteria[CriteriaNameIndex].Trim();
                CriteriaOperator = criteria[CriteriaOperatorIndex].Trim();
                var value = criteria[CriteriaValueIntIndex];
                int.TryParse(value, out CriteriaValueInt);
                float.TryParse(value, out CriteriaValueFloat);

                if (criteria.Count > 4)
                {
                    value = criteria[CriterionIDIndex].Trim();
                    int.TryParse(value, out CriterionID);
                }
                else
                {
                    CriterionID = -1;
                }
            }

            public override string ToString()
            {
                if (Math.Abs(CriteriaValueInt - CriteriaValueFloat) < float.Epsilon)
                    return CriteriaName + " " + CriteriaOperator + " " + CriteriaValueInt;

                if (Math.Abs(CriteriaValueFloat) < 0.002)
                    return CriteriaName + " " + CriteriaOperator + " " + CriteriaValueFloat.ToString("0.00E+00");

                return CriteriaName + " " + CriteriaOperator + " " + CriteriaValueFloat.ToString("0.000");
            }
        }

        protected Dictionary<string, List<FilterCriteriaDef>> m_FilterGroups = new();
        protected int m_filterSetID;
        protected Regex m_CleanSeqRegex;
        protected Regex m_CleavageStateRegex;

        protected Regex m_TerminusStateRegex;
        public enum CleavageStateTypes
        {
            Non = 0,
            Partial = 1,
            Full = 2
        }

        public FilterResultsBase(IEnumerable<string[]> filterCriteria, string filterSetID)
        {
            foreach (var criteria in filterCriteria)
            {
                var fc = new FilterCriteriaDef(criteria);

                var groupID = criteria[CriteriaGroupIDIndex];
                if (!m_FilterGroups.ContainsKey(groupID))
                {
                    m_FilterGroups[groupID] = new List<FilterCriteriaDef>();
                }
                m_FilterGroups[groupID].Add(fc);
            }
            // this.m_filters = filterCriteria;
            // this.m_FilterGroups = GetGroupList(this.m_filters);

            m_CleanSeqRegex = new Regex(@"(?<cleanseq>[a-zA-z\.]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            m_CleavageStateRegex = new Regex(@"^(?<Prefix>\S)\.(?<PepStart>\S)\S*(?<PepEnd>[A-Z])[^A-Z]*\.(?<Suffix>\S)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            m_TerminusStateRegex = new Regex(@"^(?<LeftAA>\S)\.\S+\.(?<RightAA>\S)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            int.TryParse(filterSetID, out m_filterSetID);
        }

        /// <summary>
        /// Creates a tab-delimited text file with details of the filter groups for the filter set associated with this class
        /// </summary>
        /// <param name="outputFilePath"></param>
        public void WriteCriteria(string outputFilePath)
        {
            using var writer = new StreamWriter(new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read));

            writer.WriteLine("Filter_Set_ID" + "\t" + "Filter_Criteria_Group_ID" + "\t" + "Criterion_Name" + "\t" +
                             "Operator" + "\t" + "Criterion_Value" + "\t" + "Criterion_ID");

            foreach (var filterGroupID in m_FilterGroups.Keys)
            {
                foreach (var filterRow in m_FilterGroups[filterGroupID])
                {
                    writer.Write(m_filterSetID + "\t" + filterGroupID + "\t" + filterRow.CriteriaName + "\t" +
                                 filterRow.CriteriaOperator + "\t");

                    if (Math.Abs((int)filterRow.CriteriaValueFloat - filterRow.CriteriaValueFloat) < Single.Epsilon)
                        writer.Write(filterRow.CriteriaValueInt);
                    else
                        writer.Write(filterRow.CriteriaValueFloat);

                    writer.WriteLine("\t" + filterRow.CriterionID);
                }
            }
        }

        protected bool CompareDouble(double valueToCompare, string operatorSymbol, double criterionValue)
        {
            const double thresholdNearlyEqual = float.Epsilon * 10;
            return CompareDouble(valueToCompare, operatorSymbol, criterionValue, thresholdNearlyEqual);
        }

        protected bool CompareDouble(double valueToCompare, string operatorSymbol, double criterionValue, double thresholdNearlyEqual)
        {
            var tmpDelta = Math.Abs(valueToCompare - criterionValue);

            switch (operatorSymbol)
            {
                case "<":
                    // less than
                    if (valueToCompare < criterionValue)
                        return true;
                    break;

                case "<=":
                    // less than or equal to
                    if ((valueToCompare < criterionValue) || (tmpDelta < thresholdNearlyEqual))
                        return true;
                    break;

                case "=":
                    // equal
                    if (tmpDelta < thresholdNearlyEqual)
                        return true;
                    break;

                case ">=":
                    // greater than or equal to
                    if ((valueToCompare > criterionValue) || (tmpDelta < thresholdNearlyEqual))
                        return true;
                    break;

                case ">":
                    // greater than
                    if (valueToCompare > criterionValue)
                        return true;
                    break;
            }

            return false;
        }

        protected bool CompareInteger(int valueToCompare, string operatorSymbol, int criterionValue)
        {
            switch (operatorSymbol)
            {
                case "<":
                    // less than
                    if (valueToCompare < criterionValue)
                        return true;
                    break;

                case "<=":
                    // less than or equal to
                    if (valueToCompare <= criterionValue)
                        return true;
                    break;

                case "=":
                    // equal
                    if (valueToCompare == criterionValue)
                        return true;
                    break;

                case ">=":
                    // greater than or equal to
                    if (valueToCompare >= criterionValue)
                        return true;
                    break;

                case ">":
                    // greater than
                    if (valueToCompare > criterionValue)
                        return true;
                    break;
            }

            return false;
        }

        protected CleavageStateTypes GetCleavageState(string peptideSequence)
        {
            // Implements IFilterResults.GetCleavageState

            var num = 0;

            var m = m_CleavageStateRegex.Match(peptideSequence);

            var prefix = m.Groups["Prefix"].ToString();
            var pepStart = m.Groups["PepStart"].ToString();
            var pepEnd = m.Groups["PepEnd"].ToString();
            var suffix = m.Groups["Suffix"].ToString();

            if (prefix.Equals("R") || prefix.Equals("K"))
            {
                if (!pepStart.Equals("P"))
                    num++;
            }
            else if (prefix.Equals("-"))
            {
                num++;
            }

            if (pepEnd.Equals("R") || pepEnd.Equals("K"))
            {
                if (!suffix.Equals("P"))
                    num++;
            }
            else if (suffix.Equals("-"))
            {
                num++;
            }

            return num switch
            {
                0 => CleavageStateTypes.Non,
                1 => CleavageStateTypes.Partial,
                2 => CleavageStateTypes.Full,
                // ReSharper disable once UnreachableSwitchArmDueToIntegerAnalysis
                _ => CleavageStateTypes.Full
            };
        }

        protected int GetTerminusState(string peptideSequence)
        {
            // var r = new Regex("^(?<LeftAA>\S)\.\S+\.(?<RightAA>\S)$")
            var m = m_TerminusStateRegex.Match(peptideSequence);

            if (m.Groups["LeftAA"].ToString().Equals("-") && !m.Groups["RightAA"].ToString().Equals("-"))
            {
                return 1;
            }

            if (m.Groups["RightAA"].ToString().Equals("-") && !m.Groups["LeftAA"].ToString().Equals("-"))
            {
                return 2;
            }

            if (m.Groups["RightAA"].ToString().Equals("-") && m.Groups["LeftAA"].ToString().Equals("-"))
            {
                return 3;
            }

            return 0;
        }

        protected int GetPeptideLength(string dirtySequence)
        {
            var matches = m_CleanSeqRegex.Matches(dirtySequence);

            // This will hold the letters and periods in dirtySequence
            var cleanSequence = new StringBuilder();

            foreach (Match m in matches)
            {
                cleanSequence.Append(m.Value);
            }

            if (Regex.IsMatch(cleanSequence.ToString(), @"\S\.\S+\.\S"))
            {
                var tmpSeq = Regex.Split(cleanSequence.ToString(), @"\.");
                return tmpSeq[1].Length;
            }

            return cleanSequence.Length;
        }
    }
}
