using System;
using System.IO;

namespace LabManagementSystem
{
    public static class Logger
    {
        private static readonly string LogPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        static Logger()
        {
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
        }

        private static void Write(string level, string message)
        {
            string file = Path.Combine(LogPath, $"{DateTime.Now:yyyy-MM-dd}.log");
            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

            File.AppendAllText(file, log + Environment.NewLine);
        }

        public static void LogInfo(string msg) => Write("INFO", msg);
        public static void LogWarning(string msg) => Write("WARNING", msg);
        public static void LogError(string msg, Exception ex)
        {
            Write("ERROR", $"{msg}\n{ex}");
        }
    }
}
