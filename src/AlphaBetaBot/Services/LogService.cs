using System;
using Disqord.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace AlphaBetaBot
{
    public class LogService
    {
        private readonly ILogger _logger;

        public LogService(string name)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File($"{name}.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static LogService GetLogger<T>() => new LogService(typeof(T).Name);
        public static LogService GetLogger(string name) => new LogService(name);

        public void Log(LogEventLevel level, string message, Exception e = null)
        {
            if (e is null)
                _logger.Write(level, message);
            else
                _logger.Write(level, e, message);
        }

        public void Log(LogMessageSeverity severity, string message, Exception e = null)
        {
            switch (severity)
            {
                case LogMessageSeverity.Critical:
                Fatal(message);
                break;
                case LogMessageSeverity.Debug:
                Debug(message);
                break;
                case LogMessageSeverity.Error:
                if (e is null)
                    Error(message);
                else
                    Error(message, e);
                break;
                case LogMessageSeverity.Warning:
                Warning(message);
                break;
            };
        }

        public void Log(MessageLoggedEventArgs eventArgs) => Log(eventArgs.Severity, eventArgs.ToString(), eventArgs.Exception);

        public void Trace(string message) => _logger.Verbose(message);
        public void Debug(string message) => _logger.Debug(message);
        public void Info(string message) => _logger.Information(message);
        public void Fatal(string message) => _logger.Fatal(message);
        public void Error(string message, Exception e) => _logger.Error(e, message);
        public void Error(string message) => _logger.Error(message);
        public void Warning(string message) => _logger.Warning(message);
    }
}
