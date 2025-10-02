using System.IO;

namespace WorksetCheck.Services
{
    public class Logger
    {
        private string _logFilePath = null!;
        
        public void StartLog(string? fileName)
        {
            _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + ".txt");

            File.WriteAllText(_logFilePath, $"Лог начат: {DateTime.Now}\n\n");
        }

        public void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            File.AppendAllText(_logFilePath, logEntry);
        }

        public void LogError(string message, Exception ex)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] ОШИБКА: {message}\n" + $"{ex.Message}\n" +
                              $"{ex.StackTrace}\n";
            File.AppendAllText(_logFilePath, logEntry);
        }

        /// <summary>
        /// Логирует список строк (например, ошибки проверки рабочих наборов).
        /// </summary>
        public void LogList(IEnumerable<string>? messages, string? header = null)
        {
            if (messages == null) return;

            var logText = "";

            if (!string.IsNullOrEmpty(header))
            {
                logText += $"\n=== {header} ({DateTime.Now}) ===\n";
            }

            foreach (var msg in messages)
            {
                logText += $"- {msg}\n";
            }

            File.AppendAllText(_logFilePath, logText);
        }
    }
}