using System;
using System.Windows.Forms;

// Configure log4net using the .log4net file
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Logging.config", Watch = true)]

namespace MageExtractor {

    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new ExtractorForm());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught: " + ex.Message);
				Console.WriteLine(ex.StackTrace);
				MessageBox.Show("Critical error: " + ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				var fiExe = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
				if (fiExe.FullName.StartsWith(@"\\"))
					MessageBox.Show("You are running this program from a network share.  Try copying folder " + fiExe.Directory.Name + " to your local computer and then re-running " + fiExe.Name, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
        }
    }
}
