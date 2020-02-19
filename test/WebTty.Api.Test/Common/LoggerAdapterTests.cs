using System;
using Microsoft.Extensions.Logging;
using Moq;
using WebTty.Api.Common;
using Xunit;

namespace WebTty.Api.Test.Common
{
    public class LoggerAdapterTests
    {
        private Mock<ILogger<object>> MockLogger;

        class DummyService { }
        public LoggerAdapterTests()
        {
            MockLogger = new Mock<ILogger<object>>();
        }

        [Fact]
        public void LoggerAdapter_LogDebug_WritesADebugMessage()
        {
            // Given
            var logger = new LoggerAdapter<object>(MockLogger.Object);

            // When
            logger.LogDebug("hello", 1, 2);

            // Then
            MockLogger.Verify(f => f.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void LoggerAdapter_LogInformation_WritesAnInformationMessage()
        {
            // Given
            var logger = new LoggerAdapter<object>(MockLogger.Object);

            // When
            logger.LogInformation("info", 1, 2);

            // Then
            MockLogger.Verify(f => f.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void LoggerAdapter_LogWarning_WritesAWarningMessage()
        {
            // Given
            var logger = new LoggerAdapter<object>(MockLogger.Object);

            // When
            logger.LogWarning("warning", 1, 2);

            // Then
            MockLogger.Verify(f => f.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void LoggerAdapter_LogError_WritesAnErrorMessage()
        {
            // Given
            var logger = new LoggerAdapter<object>(MockLogger.Object);

            // When
            logger.LogError("error", 1, 2);

            // Then
            MockLogger.Verify(f => f.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}
