using System;
using System.Windows.Forms;

// This program can be used to concatenate together a list of
// tab-delimited files or CSV files to form a single, combined file
//
// -------------------------------------------------------------------------------
// Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
// Program started October 23, 2014
// Copyright 2014, Battelle Memorial Institute.  All Rights Reserved.

// E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com
// Website: http://omics.pnl.gov/ or http://www.sysbio.org/resources/staff/
// -------------------------------------------------------------------------------
// 
// Licensed under the Apache License, Version 2.0; you may not use this file except
// in compliance with the License.  You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0
// Configure log4net using the .log4net file

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Logging.config", Watch = true)]

namespace MageConcatenator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MageConcatenator());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
                Console.WriteLine(ex.StackTrace.ToString());
                MessageBox.Show("Critical error: " + ex.Message + "\n" + ex.StackTrace.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                var fiExe = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (fiExe.FullName.StartsWith(@"\\"))
                    MessageBox.Show("You are running this program from a network share.  Try copying folder " + fiExe.Directory.Name + " to your local computer and then re-running " + fiExe.Name, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
