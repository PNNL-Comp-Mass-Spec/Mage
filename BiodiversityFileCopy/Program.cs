using System;

namespace BiodiversityFileCopy
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var proc = new Processing();

            if (CommandOptions.InterpretCommandLineOptions(proc, args))
            {
                proc.ProcessDataPackages();
            }
            Console.WriteLine("----");
            Console.WriteLine("Program Finished. Press any key to exit");
            Console.ReadKey();
        }
    }
}
