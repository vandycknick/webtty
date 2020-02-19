using Microsoft.Extensions.Logging;

namespace WebTty.Api.Common
{
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogDebug(string message, params object[] args) =>
            _logger.LogDebug(message, args);


        public void LogInformation(string message, params object[] args) =>
            _logger.LogInformation(message, args);

        public void LogWarning(string message, params object[] args) =>
            _logger.LogWarning(message, args);

        public void LogError(string message, params object[] args) =>
            _logger.LogError(message, args);
    }
}
