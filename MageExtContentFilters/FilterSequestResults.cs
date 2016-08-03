using System;
using System.Collections.ObjectModel;

namespace MageExtContentFilters
{

    public class FilterSequestResults : FilterResultsBase
    {

        public FilterSequestResults(Collection<string[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID)
        {

        }

        public bool EvaluateSequest(string peptideSequence, double xCorrValue, double delCNValue, double delCN2Value, int chargeState, double peptideMass, int cleavageState)
        {
            //Implements IFilterResults.EvaluatePeptide

            return this.EvaluateSequest(peptideSequence, xCorrValue, delCNValue, delCN2Value, chargeState, peptideMass, -1, -1, -1, -1, -1, -1);
        }


        public bool EvaluateSequest(string peptideSequence, double xCorrValue, double delCNValue, double delCN2Value, int chargeState, double peptideMass, int spectrumCount, double discriminantScore, double NETAbsoluteDifference, int cleavageState, double msgfSpecProb, int rankXc)
        {
            //Implements IFilterResults.EvaluatePeptide

            string currCritName = null;
            string currCritOperator = null;

            bool currEval = false;
            int peptideLength = this.GetPeptideLength(peptideSequence);
            int termState = 0;

            if (cleavageState == -1)
            {
                cleavageState = Convert.ToInt32(this.GetCleavageState(peptideSequence));
            }

            foreach (string filterGroupID in this.m_FilterGroups.Keys)
            {
                currEval = true;
                foreach (FilterCriteriaDef filterRow in m_FilterGroups[filterGroupID])
                {
                    currCritName = filterRow.CriteriaName;
                    currCritOperator = filterRow.CriteriaOperator;

                    switch (currCritName)
                    {
                        case "Charge":
                            if (chargeState > 0)
                            {
                                if (!CompareInteger(chargeState, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "MSGF_SpecProb":
                            if (msgfSpecProb > -1)
                            {
                                if (!CompareDouble(msgfSpecProb, currCritOperator, filterRow.CriteriaValueFloat))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Cleavage_State":
                            if (cleavageState > -1)
                            {
                                if (!CompareInteger(cleavageState, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Terminus_State":
                            if (termState > -1)
                            {
                                if (termState < 0)
                                    termState = this.GetTerminusState(peptideSequence);

                                if (!CompareInteger(termState, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Peptide_Length":
                            if (peptideLength > 0)
                            {
                                if (!CompareInteger(peptideLength, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Mass":
                            if (peptideMass > 0)
                            {
                                if (!CompareDouble(peptideMass, currCritOperator, filterRow.CriteriaValueFloat, 0.000001))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "High_Normalized_Score":
                            if (xCorrValue > -1)
                            {
                                if (!CompareDouble(xCorrValue, currCritOperator, filterRow.CriteriaValueFloat, 0.0001))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "DelCn":
                            if (delCNValue > -1)
                            {
                                if (!CompareDouble(delCNValue, currCritOperator, filterRow.CriteriaValueFloat, 0.0001))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "DelCn2":
                            if (delCN2Value > -1)
                            {
                                if (!CompareDouble(delCN2Value, currCritOperator, filterRow.CriteriaValueFloat, 0.0001))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Spectrum_Count":
                            if (spectrumCount > 0)
                            {
                                if (!CompareInteger(spectrumCount, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Discriminant_Score":
                            if (discriminantScore > -1)
                            {
                                if (!CompareDouble(discriminantScore, currCritOperator, filterRow.CriteriaValueFloat, 0.000001))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "NET_Difference_Absolute":
                            if (NETAbsoluteDifference > -1)
                            {
                                if (!CompareDouble(NETAbsoluteDifference, currCritOperator, filterRow.CriteriaValueFloat, 0.000001))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "RankScore":
                            if (rankXc > 0)
                            {
                                if (!CompareInteger(rankXc, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        default:
                            currEval = true;
                            break;
                    }

                    if (currEval == false)
                        break;                       //Subject didn't pass a criterion value, so move on to the next group

                }

                if (currEval == true)
                    break;                           //Subject passed the criteria for this filtergroup
            }

            return currEval;

        }

    }
}
