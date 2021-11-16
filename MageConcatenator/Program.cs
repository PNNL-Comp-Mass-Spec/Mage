using System;
using System.Windows.Forms;

// This program can be used to concatenate together a list of
// tab-delimited files or CSV files to form a single, combined file
//
// -------------------------------------------------------------------------------
// Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
// Program started October 23, 2014
// Copyright 2014, Battelle Memorial Institute.  All Rights Reserved.

// E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov
// Website: https://github.com/PNNL-Comp-Mass-Spec/ or https://panomics.pnnl.gov/ or https://www.pnnl.gov/integrative-omics
// -------------------------------------------------------------------------------
//
// Licensed under the 2-Clause BSD License; you may not use this file except
// in compliance with the License.  You may obtain a copy of the License at
// https://opensource.org/licenses/BSD-2-Clause

namespace MageConcatenator
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
                Application.Run(new MageConcatenator());
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
