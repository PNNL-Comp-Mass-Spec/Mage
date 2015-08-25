using System;
using System.Collections.ObjectModel;

namespace MageExtContentFilters {

    public class FilterMSGFDbResults : FilterResultsBase {

		public FilterMSGFDbResults(Collection<string[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID) {
        }

        public bool EvaluateMSGFDB(string peptideSequence, int chargeState, double peptideMass, double specEValue, double eValue, double FDR, double PepFDR, double msgfSpecProb, int rankMSGFDbSpecProb)
        {
		    var currEval = true;
            var peptideLength = GetPeptideLength(peptideSequence);
            var termState = 0;

            var cleavageState = Convert.ToInt32(GetCleavageState(peptideSequence));

            foreach (var filterGroupID in m_FilterGroups.Keys) {
                currEval = true;
                foreach (var filterRow in m_FilterGroups[filterGroupID]) {
                    var currCritName = filterRow.CriteriaName;
                    var currCritOperator = filterRow.CriteriaOperator;

                    switch (currCritName) {
                        case "Charge":
                            if (chargeState > 0) {
                                if (!CompareInteger(chargeState, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                }
                            }
                            break;
                        case "MSGF_SpecProb":
                            if (msgfSpecProb > -1) {
                                if (!CompareDouble(msgfSpecProb, currCritOperator, filterRow.CriteriaValueFloat)) {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Cleavage_State":
                            if (cleavageState > -1) {
                                if (!CompareInteger(cleavageState, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Terminus_State":
                            if (termState > -1) {
                            	if (termState < 0)
	                            	termState = GetTerminusState(peptideSequence);

                                if (!CompareInteger(termState, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Peptide_Length":
                            if (peptideLength > 0) {
                                if (!CompareInteger(peptideLength, currCritOperator, filterRow.CriteriaValueInt)) {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Mass":
                            if (peptideMass > 0) {
                                if (!CompareDouble(peptideMass, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                    currEval = false;
                                }
                            }
                            break;
                        case "MSGFDB_SpecProb":
                            if (specEValue > -1)
                            {
                                if (!CompareDouble(specEValue, currCritOperator, filterRow.CriteriaValueFloat))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "MSGFDB_PValue":
                            if (eValue > -1) {
                                if (!CompareDouble(eValue, currCritOperator, filterRow.CriteriaValueFloat)) {
                                    currEval = false;
                                }
                            }
                            break;
                        case "MSGFDB_FDR":
						case "MSGFPlus_QValue":
                            if (FDR > -1) {
                                if (!CompareDouble(FDR, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                    currEval = false;
                                }
                            }
                            break;
						 case "MSGFDB_PepFDR":
                            if (PepFDR > -1) {
                                if (!CompareDouble(PepFDR, currCritOperator, filterRow.CriteriaValueFloat, 0.000001)) {
                                    currEval = false;
                                }
                            }
                            break;
						 case "RankScore":
							if (rankMSGFDbSpecProb > 0)
							{
								if (!CompareInteger(rankMSGFDbSpecProb, currCritOperator, filterRow.CriteriaValueInt))
								{
									currEval = false;
								}
							}
							break;
                    }
                    
                    if (currEval == false)
                        break;                       //Subject didn't pass a criterion value, so move on to the next group

                }
                
                if (currEval)
                    break;                           //Subject passed the criteria for this filtergroup
            }

            return currEval;

        }

    }
}
