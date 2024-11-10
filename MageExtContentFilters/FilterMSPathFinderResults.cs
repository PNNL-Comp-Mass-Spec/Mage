using System;
using System.Collections.Generic;

namespace MageExtContentFilters
{
    public class FilterMSPathFinderResults : FilterResultsBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filterCriteria">Filter criteria</param>
        /// <param name="filterSetID">Filter set ID</param>
        public FilterMSPathFinderResults(IEnumerable<string[]> filterCriteria, string filterSetID)
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
                            // We don't run MSGF on MSPathFinder results, but we will test the filter threshold against specEValue
                            if (specEValue > -1 && !CompareDouble(specEValue, currentOperator, filterRow.CriteriaValueFloat))
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

                        case "MSGFDB_SpecProb":
                            if (specEValue > -1 && !CompareDouble(specEValue, currentOperator, filterRow.CriteriaValueFloat))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "MSGFDB_PValue":
                            if (eValue > -1 && !CompareDouble(eValue, currentOperator, filterRow.CriteriaValueFloat))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "MSGFDB_FDR":
                        case "MSGFPlus_QValue":
                            if (qValue > -1 && !CompareDouble(qValue, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
                            break;

                        case "MSGFDB_PepFDR":
                            if (pepQValue > -1 && !CompareDouble(pepQValue, currentOperator, filterRow.CriteriaValueFloat, 0.000001))
                            {
                                passesFilter = false;
                            }
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
