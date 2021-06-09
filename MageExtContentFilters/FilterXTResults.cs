using System;
using System.Collections.Generic;

namespace MageExtContentFilters
{
    public class FilterXTResults : FilterResultsBase
    {
        public FilterXTResults(IEnumerable<string[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID)
        {
        }

        public bool EvaluateXTandem(string peptideSequence, double hyperScoreValue, double logEValue, double delCN2Value, int chargeState, double peptideMass, double msgfSpecProb)
        {
            var passesFilter = true;
            var peptideLength = GetPeptideLength(peptideSequence);
            var terminusState = 0;

            var cleavageState = Convert.ToInt32(GetCleavageState(peptideSequence));

            foreach (var filterGroupID in m_FilterGroups.Keys)
            {
                passesFilter = true;
                foreach (var filterRow in m_FilterGroups[filterGroupID])
                {
                    var currentCriteriaName = filterRow.CriteriaName;
                    var currentOperator = filterRow.CriteriaOperator;

                    switch (currentCriteriaName)
                    {
                        case "Charge":
                            if (chargeState > 0 && !CompareInteger(chargeState, currentOperator, filterRow.CriteriaValueInt))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "MSGF_SpecProb":
                            if (msgfSpecProb > -1 && !CompareDouble(msgfSpecProb, currentOperator, filterRow.CriteriaValueFloat))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Cleavage_State":
                            if (cleavageState > -1 && !CompareInteger(cleavageState, currentOperator, filterRow.CriteriaValueInt))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Terminus_State":
                            if (terminusState > -1)
                            {
                                if (terminusState < 0)
                                    terminusState = GetTerminusState(peptideSequence);

                                if (!CompareInteger(terminusState, currentOperator, filterRow.CriteriaValueInt))
                                {
                                    passesFilter = false;
                                }
                            }
                            break;

                        case "Peptide_Length":
                            if (peptideLength > 0 && !CompareInteger(peptideLength, currentOperator, filterRow.CriteriaValueInt))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Mass":
                            if (peptideMass > 0 && !CompareDouble(peptideMass, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "XTandem_Hyperscore":
                            if (hyperScoreValue > -1 && !CompareDouble(hyperScoreValue, currentOperator, filterRow.CriteriaValueFloat, 0.0001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "XTandem_LogEValue":
                            if (!CompareDouble(logEValue, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "DelCn2":
                            if (delCN2Value > -1 && !CompareDouble(delCN2Value, currentOperator, filterRow.CriteriaValueFloat))
                            {
                                passesFilter = false;
                            }
                            break;

                        default:
                            passesFilter = true;
                            break;
                    }

                    if (!passesFilter)
                        break;                       // Subject didn't pass a criterion value, so move on to the next group
                }

                if (passesFilter)
                    break;                           // Subject passed the criteria for this filter group
            }

            return passesFilter;
        }
    }
}
