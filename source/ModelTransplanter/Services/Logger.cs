using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTransplanter.Services
{
    public class Logger
    {
        private readonly string _logFilePath;

        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
            File.WriteAllText(_logFilePath, $"Лог начат: {DateTime.Now}\n\n");
        }

        public void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            File.AppendAllText(_logFilePath, logEntry);
        }

        public void LogError(string message, Exception ex)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] ОШИБКА: {message}\n{ex.Message}\n{ex.StackTrace}\n";
            File.AppendAllText(_logFilePath, logEntry);
        }
    }
}
