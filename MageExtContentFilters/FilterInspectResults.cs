using System;
using System.Collections.Generic;

namespace MageExtContentFilters
{
    public class FilterInspectResults : FilterResultsBase
    {
        public FilterInspectResults(IEnumerable<string[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID)
        {
        }

        public bool EvaluateInspect(string peptideSequence, int chargeState, double peptideMass, double MQScore, double TotalPRMScore, double FScore, double PValue, double msgfSpecProb, int rankTotalPRMScore)
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

                        case "Inspect_MQScore":
                            if (MQScore > -1 && !CompareDouble(MQScore, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Inspect_TotalPRMScore":
                            if (TotalPRMScore > -1 && !CompareDouble(TotalPRMScore, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Inspect_FScore":
                            if (FScore > -1 && !CompareDouble(FScore, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Inspect_PValue":
                            if (PValue > -1 && !CompareDouble(PValue, currentOperator, filterRow.CriteriaValueFloat))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "RankScore":
                            if (rankTotalPRMScore > 0 && !CompareInteger(rankTotalPRMScore, currentOperator, filterRow.CriteriaValueInt))
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
