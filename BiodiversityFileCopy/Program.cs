using System;

namespace BiodiversityFileCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            var proc = new Processing();

            if (CommandOptions.InterpretCommandLineOptions(proc, args))
            {
                proc.ProcessDataPackages();
            }
            Console.WriteLine("----");
            Console.WriteLine("Program Finished.    Hit any key to exit");
            Console.ReadKey();
        }
    }
}
