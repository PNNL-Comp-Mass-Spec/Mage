using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MageExtContentFilters {

    public class FilterInspectResults : FilterResultsBase {

        public FilterInspectResults(Collection<object[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID) {
        }

        public bool EvaluateInspect(string peptideSequence, int chargeState, double peptideMass, double MQScore, double TotalPRMScore, double FScore, double PValue, double msgfSpecProb) {
            string currCritName = null;
            string currCritOperator = null;

            bool currEval = true;
            int peptideLength = this.GetPeptideLength(peptideSequence);
            int termState = 0;

            int cleavageState = Convert.ToInt32(this.GetCleavageState(peptideSequence));

            foreach (string filterGroupID in this.m_FilterGroups.Keys) {
                currEval = true;
                foreach (FilterCriteriaDef filterRow in m_FilterGroups[filterGroupID]) {
                    currCritName = filterRow.CriteriaName;
                    currCritOperator = filterRow.CriteriaOperator;

                    switch (currCritName) {
                        case "Charge":
                            if (chargeState > 0) {
                                if (!CompareInteger(chargeState, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "MSGF_SpecProb":
                            if (msgfSpecProb > -1) {
                                if (!CompareDouble(msgfSpecProb, currCritOperator, filterRow.CriteriaValueFloat)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Cleavage_State":
                            if (cleavageState > -1) {
                                if (!CompareInteger(cleavageState, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Terminus_State":
                            if (termState > -1) {
                            	if (termState < 0)
	                            	termState = this.GetTerminusState(peptideSequence);

                                if (!CompareInteger(termState, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Peptide_Length":
                            if (peptideLength > 0) {
                                if (!CompareInteger(peptideLength, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Mass":
                            if (peptideMass > 0) {
                                if (!CompareDouble(peptideMass, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Inspect_MQScore":
                            if (MQScore > -1) {
                                if (!CompareDouble(MQScore, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "Inspect_TotalPRMScore":
                            if (TotalPRMScore > -1) {
                                if (!CompareDouble(TotalPRMScore, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
						 case "Inspect_FScore":
                            if (FScore > -1) {
                                if (!CompareDouble(FScore, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;                        
                        case "Inspect_PValue":
                            if (PValue > -1) {
                                if (!CompareDouble(PValue, currCritOperator, filterRow.CriteriaValueFloat)) {
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
