using System.IO;

namespace QuantityCheck.Services;

    public class Logger
    {
        private string? _logFilePath;

        public void StartLog(string? logFilePath)
        {
            if (!string.IsNullOrEmpty(logFilePath))
            {
                _logFilePath = logFilePath;
            }
            else
            {
                _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "QuantityCheckLog.txt");
            }
            File.WriteAllText(_logFilePath, $"Лог начат: {DateTime.Now}\n\n");
        }

        public void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            if (_logFilePath != null) File.AppendAllText(_logFilePath, logEntry);
        }

        public void LogError(string message, Exception ex)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] ОШИБКА: {message}\n{ex.Message}\n{ex.StackTrace}\n";
            if (_logFilePath != null) File.AppendAllText(_logFilePath, logEntry);
        }
    }