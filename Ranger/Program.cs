using System;
using System.Windows.Forms;
using PRISM;

namespace Ranger
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new RangerForm());
            }
            catch (Exception ex)
            {
                ConsoleMsgUtils.ShowWarning("Exception caught: " + ex.Message);

                var stackTrace = StackTraceFormatter.GetExceptionStackTraceMultiLine(ex);
                ConsoleMsgUtils.ShowWarning(stackTrace);

                MessageBox.Show("Critical error: " + ex.Message + "\n" + stackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                var exeInfo = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (exeInfo.FullName.StartsWith(@"\\") && exeInfo.Directory != null)
                {
                    MessageBox.Show(string.Format(
                                        "You are running this program from a network share. " +
                                        "Try copying directory {0} to your local computer and then re-running {1}",
                                        exeInfo.Directory.Name, exeInfo.Name),
                                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
