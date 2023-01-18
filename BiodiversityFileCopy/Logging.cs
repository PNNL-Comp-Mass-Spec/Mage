using System;
using System.IO;
using Mage;

namespace BiodiversityFileCopy
{
    internal static class Logging
    {
        // ReSharper disable once CommentTypo
        // Ignore Spelling: runlog

        public static string LogRootFolder { get; set; }
        public static string LogFileLabel { get; set; } = string.Empty;

        public static void LogMsg(string msg)
        {
            StatusMessage(msg);
            File.AppendAllText(Path.Combine(LogRootFolder, string.Format("runlog{0}.txt", LogFileLabel)), msg + "\n");
        }

        public static void LogWarning(string msg)
        {
            msg = "Warning: " + msg;
            LogMsg(msg);
        }
        public static void LogError(string msg)
        {
            msg = "Error: " + msg;
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
