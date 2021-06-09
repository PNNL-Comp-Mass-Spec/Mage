using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BiodiversityFileCopy
{
    internal static class CommandOptions
    {
        // Ignore Spelling: mzid, fht, mzml

        public static bool InterpretCommandLineOptions(Processing proc, string[] args)
        {
            // Explanation of command options
            var optionDocs = new Dictionary<string, string> {
                {"-OutputFolder", @"Root folder path for output files to be copied |[Default: \\proto-11\BiodiversityLibrary]"},
                {"-AssignedOrganisms", "TSV file that contains list of data package IDs |and their corresponding assigned organism. |[Default: DMS_Organism_Stats_Annotated_MICROBES.txt]"},
                {"-DataPackages", "Comma delimited list of data package IDs to process |(Ids must be in 'AssignedOrganisms' file)"},
                {"-ProcessFiles", "Allowed values: raw,mzid,fht,mzml,fasta. [Default: (process all file types)]|Comma delimited list of file categories to process"},
                {"-Copy", "Allowed values: 'on'/'off' [Default: 'on']|If 'off', no files will actually be copied |and log file names will have 'check' suffix.|If 'on', files will be copied"},
                {"-Verbose", "Allowed values: 'on'/'off' [Default: 'off']|If 'on', details for each file will be included in log."},
                {"-SourceCheck", "Allowed values: 'on'/'off' [Default: 'on']|If 'on', check existence of source files to be copied.|If 'off', do not check.|Automatically sets '-Copy' to 'off'."}
            };
            // Option keyword-to-internal code map
            var optionKeywords = new Dictionary<string, string>
            {
                {"-OutputFolder", "output_folder"},
                {"-AssignedOrganisms", "assigned_organisms"},
                {"-ProcessFiles", "process_files"},
                {"-DataPackages", "data_packages"},
                {"-Copy", "copy_files"},
                {"-Verbose", "verbose_logging"},
                {"-SourceCheck", "source_check"}
            };

            // Dump usage
            if (args.Length == 0 || string.Equals(args[0], "-help", StringComparison.OrdinalIgnoreCase) || string.Equals(args[0], "/help", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("BiodiversityFileCopy: {0}\n", proc.GetProgramVersionNumber());

                foreach (var key in optionDocs.Keys)
                {
                    Console.WriteLine("{0}\n{1}\n", key, optionDocs[key].Replace("|", "\n"));
                }

                return false;
            }

            // Defaults
            proc.OutputRootDirectoryPath = @"\\proto-11\BiodiversityLibrary\";
            proc.InputFileRootDirectoryPath = proc.OutputRootDirectoryPath;
            proc.DataPackageListFile = "DMS_Organism_Stats_Annotated_MICROBES.txt";
            proc.DoCopyFiles = true;
            proc.DoRawCopy = true;
            proc.DoMZMLCopy = true;
            proc.DoFASTACopy = true;
            proc.DoMZIDCopy = true;
            proc.DoFHTCopy = true;
            proc.Verbose = false;
            proc.DoSourceCheck = true;

            // Process arguments
            var sa = new Stack<string>(args.Reverse());
            while (sa.Count > 0)
            {
                var opt = sa.Pop();
                if (!optionKeywords.ContainsKey(opt))
                {
                    Logging.StatusMessage(string.Format("Command option '{0}' is not recognized", opt));
                    return false;
                }
                var kw = optionKeywords[opt];
                switch (kw.Trim().ToLower())
                {
                    case "output_folder":
                        proc.OutputRootDirectoryPath = sa.Pop();
                        proc.InputFileRootDirectoryPath = proc.OutputRootDirectoryPath;
                        break;
                    case "assigned_organisms":
                        proc.DataPackageListFile = sa.Pop();
                        break;
                    case "process_files":
                        proc.DoRawCopy = false;
                        proc.DoMZMLCopy = false;
                        proc.DoFASTACopy = false;
                        proc.DoMZIDCopy = false;
                        proc.DoFHTCopy = false;
                        var fileCodes = sa.Pop().Split(',').ToList();
                        foreach (var fileCode in fileCodes)
                        {
                            switch (fileCode.Trim())
                            {
                                case "raw":
                                    proc.DoRawCopy = true;
                                    break;
                                case "mzid":
                                    proc.DoMZIDCopy = true;
                                    break;
                                case "mzml":
                                    proc.DoMZMLCopy = true;
                                    break;
                                case "fasta":
                                    proc.DoFASTACopy = true;
                                    break;
                                case "fht":
                                    proc.DoFHTCopy = true;
                                    break;
                            }
                        }
                        break;
                    case "data_packages":
                        proc.DataPackagesToProcess = sa.Pop();
                        break;
                    case "copy_files":
                        proc.DoCopyFiles = sa.Pop() == "on";
                        break;
                    case "verbose_logging":
                        proc.Verbose = sa.Pop() == "on";
                        break;
                    case "source_check":
                        proc.DoSourceCheck = sa.Pop() == "on";
                        break;
                }
            }

            Logging.LogRootFolder = proc.OutputRootDirectoryPath;
            Logging.LogMsg(string.Format("\n----[{0}]---------------", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
            Logging.LogMsg("Biodiversity File Copy");
            Logging.LogMsg(string.Format("Command options: {0}", string.Join(" ", args)));

            if (string.IsNullOrEmpty(proc.DataPackagesToProcess))
            {
                Logging.StatusMessage("No data packages to process were specified");
                return false;
            }

            // Cross-compatibility
            if (!proc.DoSourceCheck)
            {
                proc.DoCopyFiles = false;
            }
            return true;
        }
    }
}
