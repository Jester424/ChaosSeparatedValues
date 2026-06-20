using System;

namespace ChaosSeparatedValues
{
    public class Logger
    {
        private readonly string logFile;

        public Logger(AppContext appContext)
        {
            logFile = Path.Combine(
                appContext.RunName,
                $"{appContext.RunName}.log");
        }

        public void Info(string message)
        {
            Write("INFO", message, ConsoleColor.Green);
        }

        public void Error(string message)
        {
            Write("ERROR", message, ConsoleColor.Red);
        }

        private void Write(string level, string message, ConsoleColor color)
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
