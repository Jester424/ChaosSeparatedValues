using System;

namespace ChaosSeparatedValues
{
    public static class Logger
    {
        private static readonly string logDirectory = "Logs";
        private static readonly string logFile = Path.Combine(logDirectory, $"run-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");

        static Logger()
        {
            Directory.CreateDirectory(logDirectory);
        }

        public static void Info(string message)
        {
            Write("INFO", message, ConsoleColor.Green);
        }

        public static void Error(string message)
        {
            Write("ERROR", message, ConsoleColor.Red);
        }

        private static void Write(string level, string message, ConsoleColor color)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string logLine = $"{time} {level,-5} {message}";

            // Console output
            Console.Write($"{time} ");

            Console.ForegroundColor = color;
            Console.Write($"{level,-5}");
            Console.ResetColor();

            Console.WriteLine($" {message}");

            // File output
            File.AppendAllText(logFile, logLine + Environment.NewLine);
        }
    }
}
