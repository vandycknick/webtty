using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using WebTty.Api.Common;
using WebTty.Api.Messages;
using WebTty.Api.Models;
using WebTty.Exec;
using Xunit;

namespace WebTty.Api.Test
{
    public class PtyMessageHandlerTests
    {
        private Mock<IEngine> MockEngine;
        private Mock<ILoggerAdapter<PtyMessageHandler>> MockLogger;

        public PtyMessageHandlerTests()
        {
            MockEngine = new Mock<IEngine>();
            MockLogger = new Mock<ILoggerAdapter<PtyMessageHandler>>();
        }

        class DummyEvent { }

        [Fact]
        public async Task PtyMessageHandler_Handle_ReturnsUnknownMessageEventForAnUnknownMessage()
        {
            // Given
            var message = new DummyEvent();

            // When
            var handler = new PtyMessageHandler(MockEngine.Object, MockLogger.Object);
            var result = await handler.Handle(message);

            // Then
            var response = Assert.IsType<UnknownMessageEvent>(result);
            Assert.Equal(nameof(DummyEvent), response.Name);
        }

        [Fact]
        public async Task PtyMessageHandler_Handle_WritesALogMessageWhenGivenAnUnknownMessage()
        {
            // Given
            var message = new DummyEvent();

            // When
            var handler = new PtyMessageHandler(MockEngine.Object, MockLogger.Object);
            var result = await handler.Handle(message);

            // Then
            MockLogger.Verify(
                logger => logger.LogWarning("UnknownMessage {messageName}", nameof(DummyEvent)),
                Times.Once
            );
        }

        [Fact]
        public async Task PtyMessageHandler_Handle_ReturnsOpenNewTabReply()
        {
            // Given
            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };
            var request = new OpenNewTabRequest();
            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            mockProcess.SetupGet(p => p.IsRunning).Returns(true);

            MockEngine
                .Setup(e => e.StartNew()).Returns(terminal);

            MockEngine
                .Setup(e => e.TryGetProcess(terminal, out process)).Returns(true);

            // When
            var handler = new PtyMessageHandler(MockEngine.Object, MockLogger.Object);
            var result = await handler.Handle(request);

            // Then
            var reply = Assert.IsType<OpenNewTabReply>(result);
            Assert.Equal(terminal.Id, reply.Id);
        }

        [Fact]
        public async Task PtyMessageHandler_Handle_ReturnsAnErrorMessageOnOpenNewTabRequestWhenProcessIsNotFound()
        {
            // Given
            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };
            var request = new OpenNewTabRequest(id: "123", title: string.Empty);
            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            MockEngine
                .Setup(e => e.StartNew()).Returns(terminal);

            MockEngine
                .Setup(e => e.TryGetProcess(terminal, out process)).Returns(false);

            // When
            var handler = new PtyMessageHandler(MockEngine.Object, MockLogger.Object);
            var result = await handler.Handle(request);

            // Then
            var error = Assert.IsType<ErrorReply>(result);
            Assert.NotNull(error.Id);
            Assert.Equal(request.Id, error.ParentId);
            Assert.Equal($"Can't find terminal with id {terminal.Id}.", error.Message);
        }

        [Fact]
        public async Task PtyMessageHandler_Handle_LogsAnErrorWhenTheProcessIsNotFound()
        {
            // Given
            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };
            var request = new OpenNewTabRequest();
            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            MockEngine
                .Setup(e => e.StartNew()).Returns(terminal);

            MockEngine
                .Setup(e => e.TryGetProcess(terminal, out process)).Returns(false);

            // When
            var handler = new PtyMessageHandler(MockEngine.Object, MockLogger.Object);
            var result = await handler.Handle(request);

            // Then
            MockLogger.Verify(l =>
                l.LogError("Error ({messageName}): can't find terminal with id {terminalId}", nameof(ResizeTabRequest), terminal.Id));
        }

        [Fact]
        public async Task PtyMessageHandler_Handle_ResizesThePtyOnResizeTabRequest()
        {
            // Given
            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };
            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            MockEngine
                .Setup(e => e.TryGetProcess(terminal.Id, out process)).Returns(true);

            // When
            var request = new ResizeTabRequest(tabId: terminal.Id);
            var handler = new PtyMessageHandler(MockEngine.Object, MockLogger.Object);

            await handler.Handle(request);

            // Then
            mockProcess.Verify(
                p => p.SetWindowSize(24, 80)
            );
        }

        [Fact]
        public async Task PtyMessageHandler_Handle_ResizeTabRequestLogsErrorMessageWhenProcessIsNotFound()
        {
            // Given
            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };
            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            MockEngine
                .Setup(e => e.TryGetProcess(terminal.Id, out process)).Returns(false);

            // When
            var request = new ResizeTabRequest(tabId: terminal.Id);
            var handler = new PtyMessageHandler(MockEngine.Object, MockLogger.Object);

            await handler.Handle(request);

            // Then
            MockLogger.Verify(
                l => l.LogError("Error ({messageName}): can't find terminal with id {terminalId}", nameof(ResizeTabRequest), terminal.Id)
            );
        }
    }
}
