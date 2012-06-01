using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;

namespace MageExtContentFilters {

    public class FilterResultsBase {

        protected static int CriteriaGroupIDIndex = 0;
        protected static int CriteriaNameIndex = 1;
        protected static int CriteriaOperatorIndex = 2;
        protected static int CriteriaValueIntIndex = 3;
        protected static int CriterionIDIndex = 4;

        // internal class
        protected class FilterCriteriaDef {
            public string CriteriaName;
            public string CriteriaOperator;
            public int CriteriaValueInt;
            public float CriteriaValueFloat;
            public int CriterionID;

            public FilterCriteriaDef(object[] criteria) {
                CriteriaName = criteria[CriteriaNameIndex].ToString().Trim();
                CriteriaOperator = criteria[CriteriaOperatorIndex].ToString().Trim();
                string value = criteria[CriteriaValueIntIndex].ToString();
                int.TryParse(value, out CriteriaValueInt);
                float.TryParse(value, out CriteriaValueFloat);

                if (criteria.Length > 4) {
                    value = criteria[CriterionIDIndex].ToString().Trim();
                    int.TryParse(value, out CriterionID);
                } else
                    CriterionID = -1;
            }
        }

        protected Dictionary<string, List<FilterCriteriaDef>> m_FilterGroups = new Dictionary<string, List<FilterCriteriaDef>>();
        protected int m_filterSetID;
        protected Regex m_CleanSeqRegex;
        protected Regex m_CleavageStateRegex;

        protected Regex m_TerminusStateRegex;
        public enum eCleavageStates {
            Non = 0,
            Partial = 1,
            Full = 2
        }

        public FilterResultsBase(Collection<object[]> filterCriteria, string filterSetID) {
            foreach (object[] criteria in filterCriteria) {
                FilterCriteriaDef fc = new FilterCriteriaDef(criteria);

                string groupID = criteria[CriteriaGroupIDIndex].ToString();
                if (!m_FilterGroups.ContainsKey(groupID)) {
                    m_FilterGroups[groupID] = new List<FilterCriteriaDef>();
                }
                m_FilterGroups[groupID].Add(fc);
            }
            //            this.m_filters = filterCriteria;
            //            this.m_FilterGroups = GetGroupList(this.m_filters);
            this.m_CleanSeqRegex = new Regex("(?<cleanseq>[a-zA-z\\.]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            this.m_CleavageStateRegex = new Regex("^(?<1>\\S)\\.(?<2>\\S)\\S*(?<3>[A-Z])[^A-Z]*\\.(?<4>\\S)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            this.m_TerminusStateRegex = new Regex("^(?<LeftAA>\\S)\\.\\S+\\.(?<RightAA>\\S)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            int.TryParse(filterSetID, out m_filterSetID);
        }

        /// <summary>
        /// Creates a tab-delimited text file with details of the filter groups for the filter set associated with this class
        /// </summary>
        /// <param name="sOutputFilePath"></param>
        public void WriteCriteria(string sOutputFilePath) {

            System.IO.StreamWriter swOutfile;

            swOutfile = new System.IO.StreamWriter(new System.IO.FileStream(sOutputFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read));

            swOutfile.WriteLine("Filter_Set_ID" + "\t" + "Filter_Criteria_Group_ID" + "\t" + "Criterion_Name" + "\t" + "Operator" + "\t" + "Criterion_Value" + "\t" + "Criterion_ID");

            foreach (string filterGroupID in this.m_FilterGroups.Keys) {
                
                foreach (FilterCriteriaDef filterRow in m_FilterGroups[filterGroupID]) {

                    swOutfile.Write(m_filterSetID + "\t" + filterGroupID + "\t" + filterRow.CriteriaName + "\t" + filterRow.CriteriaOperator + "\t");
                    ;
					if ((int)filterRow.CriteriaValueFloat == filterRow.CriteriaValueFloat)
                        swOutfile.Write(filterRow.CriteriaValueInt);
                    else
                        swOutfile.Write(filterRow.CriteriaValueFloat);

                    swOutfile.WriteLine("\t" + filterRow.CriterionID);
                }
            }

            swOutfile.Close();
        }

        protected bool CompareDouble(double valueToCompare, string operatorSymbol, double criterionValue) {
            double thresholdNearlyEqual = float.Epsilon * 10;
            return CompareDouble(valueToCompare, operatorSymbol, criterionValue, thresholdNearlyEqual);
        }

        protected bool CompareDouble(double valueToCompare, string operatorSymbol, double criterionValue, double thresholdNearlyEqual) {

            bool tmpvalue = false;
            double tmpDelta = Math.Abs(valueToCompare - criterionValue);

            switch (operatorSymbol) {
                case "<":
                    //less than
                    if (valueToCompare < criterionValue)
                        tmpvalue = true;
                    break;
                case "<=":
                    //less than or equal to
                    if (((valueToCompare < criterionValue) | (tmpDelta < thresholdNearlyEqual)))
                        tmpvalue = true;
                    break;
                case "=":
                    //equal
                    if (tmpDelta < thresholdNearlyEqual)
                        tmpvalue = true;
                    break;
                case ">=":
                    //greater than or equal to
                    if (((valueToCompare > criterionValue) | (tmpDelta < thresholdNearlyEqual)))
                        tmpvalue = true;
                    break;
                case ">":
                    //greater than
                    if (valueToCompare > criterionValue)
                        tmpvalue = true;
                    break;
                default:
                    tmpvalue = false;

                    break;
            }

            return tmpvalue;

        }

        protected bool CompareInteger(int valueToCompare, string operatorSymbol, int criterionValue) {

            bool tmpvalue = false;

            switch (operatorSymbol) {
                case "<":
                    //less than
                    if (valueToCompare < criterionValue)
                        tmpvalue = true;
                    break;
                case "<=":
                    //less than or equal to
                    if (valueToCompare <= criterionValue)
                        tmpvalue = true;
                    break;
                case "=":
                    //equal
                    if (valueToCompare == criterionValue)
                        tmpvalue = true;
                    break;
                case ">=":
                    //greater than or equal to
                    if (valueToCompare >= criterionValue)
                        tmpvalue = true;
                    break;
                case ">":
                    //greater than
                    if (valueToCompare > criterionValue)
                        tmpvalue = true;
                    break;
                default:
                    tmpvalue = false;

                    break;
            }

            return tmpvalue;

        }

        protected ArrayList GetGroupList(DataTable filterCriteria) {
            int currGroupID = 0;
            int prevGroupID = 0;

            ArrayList tmpList = new ArrayList();

            foreach (DataRow dr in filterCriteria.Rows) {
                currGroupID = Convert.ToInt32(dr["Filter_Criteria_Group_ID"]);
                if (currGroupID != prevGroupID) {
                    tmpList.Add(currGroupID);
                }
                prevGroupID = currGroupID;
            }

            return tmpList;
        }

        protected eCleavageStates GetCleavageState(string peptideSequence) {
            //Implements IFilterResults.GetCleavageState
            eCleavageStates tmpState = default(eCleavageStates);

            int num = 0;
            //Dim r As New Regex("^(?<1>\S)\.(?<2>\S)\S+(?<3>\S)\.(?<4>\S)$")
            //Dim r As New Regex("^(?<1>\S)\.(?<2>\S)\S*(?<3>[A-Ja-j,L-Ql-q,S-Zs-z])[^A-Za-z]*\.(?<4>\S)$")

            Match m = default(Match);

            m = this.m_CleavageStateRegex.Match(peptideSequence);

            string AA1 = m.Groups[1].ToString();
            string AA2 = m.Groups[2].ToString();
            string AA3 = m.Groups[3].ToString();
            string AA4 = m.Groups[4].ToString();

            if (AA1.Equals("R") | AA1.Equals("K")) {
                if (!AA2.Equals("P"))
                    num += 1;
            } else if (AA1.Equals("-")) {
                num += 1;
            }

            if (AA3.Equals("R") | AA3.Equals("K")) {
                if (!AA4.Equals("P"))
                    num += 1;
            } else if (AA4.Equals("-")) {
                num += 1;
            }

            switch (num) {
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

        protected int GetTerminusState(string peptideSequence) {
            //Dim r As New Regex("^(?<LeftAA>\S)\.\S+\.(?<RightAA>\S)$")
            Match m = this.m_TerminusStateRegex.Match(peptideSequence);
            int num = 0;

            if ((m.Groups["LeftAA"].ToString().Equals("-") & !m.Groups["RightAA"].ToString().Equals("-"))) {
                num = 1;
            } else if ((m.Groups["RightAA"].ToString().Equals("-") & !m.Groups["LeftAA"].ToString().Equals("-"))) {
                num = 2;
            } else if ((m.Groups["RightAA"].ToString().Equals("-") & m.Groups["LeftAA"].ToString().Equals("-"))) {
                num = 3;
            } else {
                num = 0;
            }

            return num;
        }

        protected int GetPeptideLength(string dirtySequence) {

            MatchCollection matches = this.m_CleanSeqRegex.Matches(dirtySequence);

            StringBuilder intermedSeq = new StringBuilder();

            foreach (Match m in matches) {
                intermedSeq.Append(m.Value);
            }

            if (Regex.IsMatch(intermedSeq.ToString(), "\\S\\.\\S+\\.\\S")) {
                string[] tmpSeq = null;
                tmpSeq = Regex.Split(intermedSeq.ToString(), "\\.");
                return tmpSeq[1].Length;
            } else {
                return intermedSeq.Length;
            }



        }

    }
}
