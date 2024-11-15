using Mage;
using System;
using System.Windows.Forms;

namespace MageFileProcessor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Ignore Spelling: Mage

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Uncomment to debug querying view V_Custom_Factors_List_Report and calling procedure predefined_analysis_rules_proc
                // (to see the console messages, change the Output Type of this project to Console Application)

                /*
                var queryTests = new DatabaseQueryTests();

                queryTests.QueryDatasetFactorsNamedUser(Globals.DMSServer, Globals.DMSDatabase, Globals.PostgresDMS, DatabaseQueryTests.DMS_READER, DatabaseQueryTests.DMS_READER_PASSWORD);

                queryTests.DMSSprocReadTest(Globals.DMSServer, Globals.DMSDatabase, Globals.PostgresDMS, DatabaseQueryTests.DMS_READER, DatabaseQueryTests.DMS_READER_PASSWORD);
                */

                Application.Run(new FileProcessorForm());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Critical error: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                var fiExe = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (fiExe.FullName.StartsWith(@"\\") && fiExe.Directory != null)
                {
                    MessageBox.Show(string.Format(
                                        "You are running this program from a network share. " +
                                        "Try copying directory {0} to your local computer and then re-running {1}",
                                        fiExe.Directory.Name, fiExe.Name),
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
