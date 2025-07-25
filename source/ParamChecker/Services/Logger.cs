﻿using System.IO;

namespace ParamChecker.Services;

public class Logger
{
    private string _logFilePath;
    
    public void StartLog(string logFilePath)
    {
        if (!string.IsNullOrEmpty(logFilePath))
        {
            _logFilePath = logFilePath;
        }
        else
        {
            _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ParamCheckerLog.txt");
        }
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