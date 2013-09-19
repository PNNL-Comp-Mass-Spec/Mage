using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MageExtContentFilters {

    public class FilterXTResults : FilterResultsBase {

		public FilterXTResults(Collection<string[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID) {
        }

        public bool EvaluateXTandem(string peptideSequence, double hyperScoreValue, double logEValue, double delCN2Value, int chargeState, double peptideMass, double msgfSpecProb) {
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
                        case "XTandem_Hyperscore":
                            if (hyperScoreValue > -1) {
                                if (!CompareDouble(hyperScoreValue, currCritOperator, filterRow.CriteriaValueFloat, 0.0001)) {
                                    currEval = false;
                                    break;
                                }
                            }
                            break;
                        case "XTandem_LogEValue":
                            if (!CompareDouble(logEValue, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                currEval = false;
                                break;                            
                            }
                            break;
                        case "DelCn2":
                            if (delCN2Value > -1) {
                                if (!CompareDouble(delCN2Value, currCritOperator, filterRow.CriteriaValueFloat)) {
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
