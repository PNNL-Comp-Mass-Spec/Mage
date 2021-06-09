using System;
using System.Collections.Generic;

namespace MageExtContentFilters
{
    public class FilterSequestResults : FilterResultsBase
    {
        public FilterSequestResults(IEnumerable<string[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID)
        {
        }

        public bool EvaluateSequest(string peptideSequence, double xCorrValue, double delCNValue, double delCN2Value, int chargeState, double peptideMass, int cleavageState)
        {
            // Implements IFilterResults.EvaluatePeptide

            return EvaluateSequest(peptideSequence, xCorrValue, delCNValue, delCN2Value, chargeState, peptideMass, -1, -1, -1, cleavageState, -1, -1);
        }

        public bool EvaluateSequest(string peptideSequence, double xCorrValue, double delCNValue, double delCN2Value, int chargeState, double peptideMass, int spectrumCount, double discriminantScore, double NETAbsoluteDifference, int cleavageState, double msgfSpecProb, int rankXc)
        {
            // Implements IFilterResults.EvaluatePeptide

            var passesFilter = false;
            var peptideLength = GetPeptideLength(peptideSequence);
            var terminusState = 0;

            if (cleavageState == -1)
            {
                cleavageState = Convert.ToInt32(GetCleavageState(peptideSequence));
            }

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

                        case "High_Normalized_Score":
                            if (xCorrValue > -1 && !CompareDouble(xCorrValue, currentOperator, filterRow.CriteriaValueFloat, 0.0001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "DelCn":
                            if (delCNValue > -1 && !CompareDouble(delCNValue, currentOperator, filterRow.CriteriaValueFloat, 0.0001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "DelCn2":
                            if (delCN2Value > -1 && !CompareDouble(delCN2Value, currentOperator, filterRow.CriteriaValueFloat, 0.0001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Spectrum_Count":
                            if (spectrumCount > 0 && !CompareInteger(spectrumCount, currentOperator, filterRow.CriteriaValueInt))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "Discriminant_Score":
                            if (discriminantScore > -1 && !CompareDouble(discriminantScore, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "NET_Difference_Absolute":
                            if (NETAbsoluteDifference > -1 && !CompareDouble(NETAbsoluteDifference, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "RankScore":
                            if (rankXc > 0 && !CompareInteger(rankXc, currentOperator, filterRow.CriteriaValueInt))
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
