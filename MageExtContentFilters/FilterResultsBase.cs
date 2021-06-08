using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MageExtContentFilters
{
    public class FilterResultsBase
    {
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

            public FilterCriteriaDef(string[] criteria)
            {
                CriteriaName = criteria[CriteriaNameIndex].Trim();
                CriteriaOperator = criteria[CriteriaOperatorIndex].Trim();
                var value = criteria[CriteriaValueIntIndex];
                int.TryParse(value, out CriteriaValueInt);
                float.TryParse(value, out CriteriaValueFloat);

                if (criteria.Length > 4)
                {
                    value = criteria[CriterionIDIndex].Trim();
                    int.TryParse(value, out CriterionID);
                }
                else
                    CriterionID = -1;
            }

            public override string ToString()
            {
                if (Math.Abs(CriteriaValueInt - CriteriaValueFloat) < Single.Epsilon)
                    return CriteriaName + " " + CriteriaOperator + " " + CriteriaValueInt;
                else
                {
                    if (Math.Abs(CriteriaValueFloat) < 0.002)
                        return CriteriaName + " " + CriteriaOperator + " " + CriteriaValueFloat.ToString("0.00E+00");
                    else
                        return CriteriaName + " " + CriteriaOperator + " " + CriteriaValueFloat.ToString("0.000");
                }
            }
        }

        protected Dictionary<string, List<FilterCriteriaDef>> m_FilterGroups = new();
        protected int m_filterSetID;
        protected Regex m_CleanSeqRegex;
        protected Regex m_CleavageStateRegex;

        protected Regex m_TerminusStateRegex;
        public enum eCleavageStates
        {
            Non = 0,
            Partial = 1,
            Full = 2
        }

        public FilterResultsBase(Collection<string[]> filterCriteria, string filterSetID)
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
        /// <param name="sOutputFilePath"></param>
        public void WriteCriteria(string sOutputFilePath)
        {
            using (var swOutfile = new StreamWriter(new FileStream(sOutputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                swOutfile.WriteLine("Filter_Set_ID" + "\t" + "Filter_Criteria_Group_ID" + "\t" + "Criterion_Name" + "\t" +
                                    "Operator" + "\t" + "Criterion_Value" + "\t" + "Criterion_ID");

                foreach (var filterGroupID in m_FilterGroups.Keys)
                {
                    foreach (var filterRow in m_FilterGroups[filterGroupID])
                    {
                        swOutfile.Write(m_filterSetID + "\t" + filterGroupID + "\t" + filterRow.CriteriaName + "\t" +
                                        filterRow.CriteriaOperator + "\t");

                        if (Math.Abs((int)filterRow.CriteriaValueFloat - filterRow.CriteriaValueFloat) < Single.Epsilon)
                            swOutfile.Write(filterRow.CriteriaValueInt);
                        else
                            swOutfile.Write(filterRow.CriteriaValueFloat);

                        swOutfile.WriteLine("\t" + filterRow.CriterionID);
                    }
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
            var tmpvalue = false;
            var tmpDelta = Math.Abs(valueToCompare - criterionValue);

            switch (operatorSymbol)
            {
                case "<":
                    // less than
                    if (valueToCompare < criterionValue)
                        tmpvalue = true;
                    break;
                case "<=":
                    // less than or equal to
                        tmpvalue = true;
                    if ((valueToCompare < criterionValue) | (tmpDelta < thresholdNearlyEqual))
                        return true;
                    break;
                case "=":
                    // equal
                    if (tmpDelta < thresholdNearlyEqual)
                        tmpvalue = true;
                    break;
                case ">=":
                    // greater than or equal to
                        tmpvalue = true;
                    if ((valueToCompare > criterionValue) | (tmpDelta < thresholdNearlyEqual))
                    break;
                case ">":
                    // greater than
                    if (valueToCompare > criterionValue)
                        tmpvalue = true;
                    break;
            }

            return tmpvalue;
        }

        protected bool CompareInteger(int valueToCompare, string operatorSymbol, int criterionValue)
        {
            var tmpvalue = false;

            switch (operatorSymbol)
            {
                case "<":
                    // less than
                    if (valueToCompare < criterionValue)
                        tmpvalue = true;
                    break;
                case "<=":
                    // less than or equal to
                    if (valueToCompare <= criterionValue)
                        tmpvalue = true;
                    break;
                case "=":
                    // equal
                    if (valueToCompare == criterionValue)
                        tmpvalue = true;
                    break;
                case ">=":
                    // greater than or equal to
                    if (valueToCompare >= criterionValue)
                        tmpvalue = true;
                    break;
                case ">":
                    // greater than
                    if (valueToCompare > criterionValue)
                        tmpvalue = true;
                    break;
            }

            return tmpvalue;
        }

        protected ArrayList GetGroupList(DataTable filterCriteria)
        {
            var prevGroupID = 0;

            var tmpList = new ArrayList();

            foreach (DataRow dr in filterCriteria.Rows)
            {
                var currGroupID = Convert.ToInt32(dr["Filter_Criteria_Group_ID"]);
                if (currGroupID != prevGroupID)
                {
                    tmpList.Add(currGroupID);
                }
                prevGroupID = currGroupID;
            }

            return tmpList;
        }

        protected eCleavageStates GetCleavageState(string peptideSequence)
        {
            // Implements IFilterResults.GetCleavageState
            eCleavageStates tmpState;

            var num = 0;
            // var r = new Regex("^(?<1>\S)\.(?<2>\S)\S+(?<3>\S)\.(?<4>\S)$")
            // var r = new Regex("^(?<1>\S)\.(?<2>\S)\S*(?<3>[A-Ja-j,L-Ql-q,S-Zs-z])[^A-Za-z]*\.(?<4>\S)$")

            var m = m_CleavageStateRegex.Match(peptideSequence);

            var prefix = m.Groups["Prefix"].ToString();
            var pepStart = m.Groups["PepStart"].ToString();
            var pepEnd = m.Groups["PepEnd"].ToString();
            var suffix = m.Groups["Suffix"].ToString();

            if (prefix.Equals("R") | prefix.Equals("K"))
            {
                if (!pepStart.Equals("P"))
                    num += 1;
            }
            else if (prefix.Equals("-"))
            {
                num += 1;
            }

            if (pepEnd.Equals("R") | pepEnd.Equals("K"))
            {
                if (!suffix.Equals("P"))
                    num += 1;
            }
            else if (suffix.Equals("-"))
            {
                num += 1;
            }

            switch (num)
            {
                case 0:
                    tmpState = eCleavageStates.Non;
                    break;
                case 1:
                    tmpState = eCleavageStates.Partial;
                    break;
                case 2:
                    tmpState = eCleavageStates.Full;
                    break;
                default:
                    tmpState = eCleavageStates.Full;
                    break;
            }

            return tmpState;
        }

        protected int GetTerminusState(string peptideSequence)
        {
            // var r = new Regex("^(?<LeftAA>\S)\.\S+\.(?<RightAA>\S)$")
            var m = m_TerminusStateRegex.Match(peptideSequence);
            int num;

            if (m.Groups["LeftAA"].ToString().Equals("-") & !m.Groups["RightAA"].ToString().Equals("-"))
            {
                num = 1;
            }
            else if (m.Groups["RightAA"].ToString().Equals("-") & !m.Groups["LeftAA"].ToString().Equals("-"))
            {
                num = 2;
            }
            else if (m.Groups["RightAA"].ToString().Equals("-") & m.Groups["LeftAA"].ToString().Equals("-"))
            {
                num = 3;
            }
            else
            {
                num = 0;
            }

            return num;
        }

        protected int GetPeptideLength(string dirtySequence)
        {
            var matches = m_CleanSeqRegex.Matches(dirtySequence);

            var intermedSeq = new StringBuilder();

            foreach (Match m in matches)
            {
                intermedSeq.Append(m.Value);
            }

            if (Regex.IsMatch(intermedSeq.ToString(), @"\S\.\S+\.\S"))
            {
                var tmpSeq = Regex.Split(intermedSeq.ToString(), @"\.");
                return tmpSeq[1].Length;
            }
            else
            {
                return intermedSeq.Length;
            }
        }
    }
}
