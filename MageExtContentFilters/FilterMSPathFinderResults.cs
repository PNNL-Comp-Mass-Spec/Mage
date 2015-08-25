using System;
using System.Collections.ObjectModel;

namespace MageExtContentFilters
{
    public class FilterMSPathFinderResults : FilterResultsBase
    {

        public FilterMSPathFinderResults(Collection<string[]> filterCriteria, string filterSetID)
            : base(filterCriteria, filterSetID)
        {
        }

        public bool EvaluateMSPathFinder(
          string peptideSequence,
          int chargeState,
          double peptideMass,
          double specEValue,
          double eValue,
          double qValue,
          double pepQValue
         )
        {
            var currEval = true;
            var peptideLength = GetPeptideLength(peptideSequence);
            var termState = 0;

            var cleavageState = Convert.ToInt32(GetCleavageState(peptideSequence));

            foreach (var filterGroupID in m_FilterGroups.Keys)
            {
                currEval = true;
                foreach (var filterRow in m_FilterGroups[filterGroupID])
                {
                    var currCritName = filterRow.CriteriaName;
                    var currCritOperator = filterRow.CriteriaOperator;

                    switch (currCritName)
                    {
                        case "Charge":
                            if (chargeState > 0)
                            {
                                if (!CompareInteger(chargeState, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "MSGF_SpecProb":
                            // We don't run MSGF on MSPathFinder results, but we will test the filter threshold against specEValue
                            if (specEValue > -1)
                            {
                                if (!CompareDouble(specEValue, currCritOperator, filterRow.CriteriaValueFloat))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Cleavage_State":
                            if (cleavageState > -1)
                            {
                                if (!CompareInteger(cleavageState, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Terminus_State":
                            if (termState > -1)
                            {
                                if (termState < 0)
                                    termState = GetTerminusState(peptideSequence);

                                if (!CompareInteger(termState, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Peptide_Length":
                            if (peptideLength > 0)
                            {
                                if (!CompareInteger(peptideLength, currCritOperator, filterRow.CriteriaValueInt))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "Mass":
                            if (peptideMass > 0)
                            {
                                if (!CompareDouble(peptideMass, currCritOperator, filterRow.CriteriaValueFloat, 0.000001))
                                {
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
                            if (eValue > -1)
                            {
                                if (!CompareDouble(eValue, currCritOperator, filterRow.CriteriaValueFloat))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "MSGFDB_FDR":
                        case "MSGFPlus_QValue":
                            if (qValue > -1)
                            {
                                if (!CompareDouble(qValue, currCritOperator, filterRow.CriteriaValueFloat, 0.000001))
                                {
                                    currEval = false;
                                }
                            }
                            break;
                        case "MSGFDB_PepFDR":
                            if (pepQValue > -1)
                            {
                                if (!CompareDouble(pepQValue, currCritOperator, filterRow.CriteriaValueFloat, 0.000001))
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
