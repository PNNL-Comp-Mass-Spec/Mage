using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mage
{

    /// <summary>
    /// Mage module that can generate a simulated data stream on Mage standard tabular output
    /// or accept a set of data that it will stream to ouput
    /// </summary>
    public class DataGenerator : BaseModule
    {

        #region Member Variables

        private readonly List<string[]> mAdHocRows = new List<string[]>();

        #endregion

        #region Properties

        /// <summary>
        /// Add a data row to the internal row buffer
        /// </summary>
        public string[] AddAdHocRow { set => mAdHocRows.Add(value); }

        /// <summary>
        /// Get the contents of the internal row buffer
        /// </summary>
        public Collection<string[]> AdHocRows => new Collection<string[]>(mAdHocRows);

        /// <summary>
        /// Include header row in generated data
        /// </summary>
        public bool IncludeHeaderInOutput { get; set; }

        /// <summary>
        /// Number of rows in generated data
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Number of columns in generated data
        /// </summary>
        public int Cols { get; set; }

        /// <summary>
        /// Template to use for fields in generated header row
        /// (must be valid for string.Format)
        /// </summary>
        public static string SimulatedHeaderTemplate { get; }

        /// <summary>
        /// Template to use for fields in generated rows
        /// (must be valid for string.Format)
        /// </summary>
        public static string SimulatedDataTemplate { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static DataGenerator()
        {
            SimulatedHeaderTemplate = "Column {0} Header";
            SimulatedDataTemplate = "Data for row-{0} column-{1}";
        }

        /// <summary>
        /// Construct a new Mage data generator object
        /// </summary>
        public DataGenerator()
        {
            Initialize();
        }

        /// <summary>
        /// Construct a new Mage data generator object
        /// with Rows and Cols properties set
        /// </summary>
        public DataGenerator(int rows, int cols)
        {
            Initialize();
            Rows = rows;
            Cols = cols;
        }

        private void Initialize()
        {
            IncludeHeaderInOutput = true;
            Rows = 20;
            Cols = 5;
        }
        #endregion


        #region IBaseModule Members

        /// <summary>
        /// Pass execution to module instead of having it respond to standard tabular input stream events
        /// (override of base class)
        /// </summary>
        /// <param name="state">Mage ProcessingPipeline object that contains the module (if there is one)</param>
        public override void Run(object state)
        {
            if (mAdHocRows.Count > 0)
            {
                OutputAdHocRows();
            }
            else
            {
                OutputGeneratedRows();
            }
        }

        private void OutputAdHocRows()
        {
            var hx = IncludeHeaderInOutput;
            foreach (var row in mAdHocRows)
            {
                if (hx)
                {
                    hx = false;
                    OutputHeaderLine(row);
                }
                else
                {
                    OutputDataLine(row);
                }
            }
            OutputDataLine(null);
        }

        private void OutputGeneratedRows()
        {
            for (var i = 0; i < Rows; i++)
            {
                string[] fields;
                if (i == 0 && IncludeHeaderInOutput)
                {
                    fields = MakeSimulatedHeaderRow(Cols);
                    OutputHeaderLine(fields);
                }
                fields = MakeSimulatedDataRow(i, Cols);
                OutputDataLine(fields);
            }
            OutputDataLine(null);
        }

        /// <summary>
        /// Return a simulted row of data with the given number of columns
        /// </summary>
        /// <param name="numCols"></param>
        /// <returns></returns>
        public static string[] MakeSimulatedHeaderRow(int numCols)
        {
            var row = new string[numCols];
            for (var j = 0; j < numCols; j++)
            {
                row[j] = string.Format(SimulatedHeaderTemplate, j + 1);
            }
            return row;
        }

        /// <summary>
        /// Return a simulated row of data based on the given row number and number of columns
        /// </summary>
        /// <param name="i"></param>
        /// <param name="numCols"></param>
        /// <returns></returns>
        public static string[] MakeSimulatedDataRow(int i, int numCols)
        {
            var row = new string[numCols];
            for (var j = 0; j < numCols; j++)
            {
                row[j] = string.Format(SimulatedDataTemplate, i + 1, j + 1);
            }
            return row;
        }

        private void OutputHeaderLine(IEnumerable<string> fields)
        {
            // Output the column definitions
            var columnDefs = new List<MageColumnDef>();
            foreach (var field in fields)
            {
                var colDef = new MageColumnDef(field, "text", "10");
                columnDefs.Add(colDef);
            }
            OnColumnDefAvailable(new MageColumnEventArgs(columnDefs.ToArray()));
        }

        private void OutputDataLine(string[] fields)
        {
            OnDataRowAvailable(new MageDataEventArgs(fields));
        }

        #endregion

    }

}
