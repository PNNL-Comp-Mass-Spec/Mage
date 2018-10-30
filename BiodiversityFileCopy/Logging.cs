using System;
using System.IO;
using Mage;


namespace BiodiversityFileCopy
{
    static class Logging
    {
        public static string LogRootFolder { get; set; }
        private static string _logFileLabel = "";
        public static string LogFileLabel
        {
            get { return _logFileLabel; }
            set { _logFileLabel = value; }
        }

        public static void LogMsg(string msg)
        {
            StatusMessage(msg);
            File.AppendAllText(Path.Combine(LogRootFolder, string.Format("runlog{0}.txt", LogFileLabel)), msg + "\n");
        }

        public static void LogWarning(string msg)
        {
            msg = "Warning:" + msg;
            LogMsg(msg);
        }
        public static void LogError(string msg)
        {
            msg = "Error:" + msg;
            LogMsg(msg);
        }

        public static void StatusMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void HandleLogMessage(object sender, MageStatusEventArgs args)
        {
            LogMsg(args.Message);
        }

        public static void HandleStatusMessage(object sender, MageStatusEventArgs args)
        {
            StatusMessage(args.Message);
        }
    }
}
