using System;
using System.IO;

namespace Tools
{
    public class LogManager
    {
        // relative root folder for logs
        private static readonly string s_logRoot = "Log";
        private static readonly object s_lock = new();

        // returns path to current month folder (relative to app base directory)
        public static string GetCurrentFolderPath()
        {
            var now = DateTime.Now;
            string monthFolder = now.ToString("yyyy-MM");
            string root = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            return Path.Combine(root, s_logRoot, monthFolder);
        }

        // returns path to today's log file (full path)
        public static string GetCurrentFilePath()
        {
            var folder = GetCurrentFolderPath();
            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            return Path.Combine(folder, fileName);
        }

        // write a formatted log line to today's file, ensure directories exist
        public static void Log(string project, string funcName, string message)
        {
            if (project == null) project = string.Empty;
            if (funcName == null) funcName = string.Empty;
            if (message == null) message = string.Empty;

            string folder = GetCurrentFolderPath();
            string filePath = GetCurrentFilePath();

            lock (s_lock)
            {
                Directory.CreateDirectory(folder);
                string line = $"{DateTime.Now}\t{project}. {funcName} : \t {message}{Environment.NewLine}";
                File.AppendAllText(filePath, line);
            }
        }
    }
}
