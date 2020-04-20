using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using WebTty.Api.Infrastructure.Protocol;
using WebTty.Api.Models;
using WebTty.Exec;
using WebTty.Integration.Test.Helpers;
using WebTty.Schema.Messages;
using Xunit;

namespace WebTty.Integration.Test
{
    public class MessagingTests : IClassFixture<WebTtyTestHost>, IAsyncLifetime
    {
        private readonly TimeSpan timeOut = TimeSpan.FromSeconds(5);
        private readonly WebTtyTestHost _testHost;
        private WebSocket socket;
        private readonly IMessageWriter writer;
        private readonly IMessageReader reader;

        public MessagingTests(WebTtyTestHost testHost)
        {
            _testHost = testHost;
            writer = _testHost.GetRequiredService<IMessageWriter>();
            reader = _testHost.GetRequiredService<IMessageReader>();
        }

        public async Task InitializeAsync()
        {
            socket = await _testHost.OpenWebSocket("pty");
        }

        public Task DisposeAsync() => Task.CompletedTask;


        [Fact]
        public async Task PtyEndpoint_Handles_OpenNewTabRequest()
        {
            // Given
            var mockEngine = _testHost.MockEngine;

            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };

            mockEngine
                .Setup(engine => engine.StartNew()).Returns(terminal);

            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            mockProcess.SetupGet(p => p.IsRunning).Returns(true);

            mockEngine
                .Setup(engine => engine.TryGetProcess(terminal, out process))
                .Returns(true);

            // When
            using var cts = new CancellationTokenSource(timeOut);
            var request = new OpenNewTabRequest(id: "456", title: "title");
            await socket.SendBinaryMessageAsync(writer, request, cts.Token);

            var response = await socket.ReceiveMessageAsync(reader, cts.Token);

            // Then
            var newTab = Assert.IsType<OpenNewTabReply>(response);
            Assert.Equal(terminal.Id, newTab.Id);
            Assert.Equal(request.Id, newTab.ParentId);
            mockEngine.Verify(engine => engine.StartNew());
        }

        [Fact]
        public async Task PtyEndpoint_Handles_ResizeTabRequest()
        {
            // Given
            var mockEngine = _testHost.MockEngine;

            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };
            var request = new ResizeTabRequest(terminal.Id, 100, 300);

            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            var reset = new AutoResetEvent(false);
            mockEngine
                .Setup(engine => engine.TryGetProcess(terminal.Id, out process))
                .Returns(true);

            mockProcess
                .Setup(process => process.SetWindowSize(request.Rows, request.Cols))
                .Callback(() => reset.Set());

            // When
            using var cts = new CancellationTokenSource(timeOut);
            await socket.SendBinaryMessageAsync(writer, request, cts.Token);
            reset.WaitOne(TimeSpan.FromSeconds(5));

            // Then
            mockProcess.Verify(
                process => process.SetWindowSize(request.Rows, request.Cols),
                Times.Once()
            );
        }

        [Fact]
        public async Task PtyEndpoint_Handles_SendInputRequest()
        {
            // Given
            var mockEngine = _testHost.MockEngine;

            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };

            var reset = new AutoResetEvent(false);

            // When
            using var cts = new CancellationTokenSource(timeOut);
            var request = new SendInputRequest(terminal.Id, "echo 'hello'");

            await socket.SendBinaryMessageAsync(writer, request, cts.Token);
            reset.WaitOne(TimeSpan.FromSeconds(1));

            // Then
            mockEngine.Verify(
                engine => engine.Write(
                    request.TabId,
                    It.Is<ReadOnlyMemory<char>>(m => m.ToString() == "echo 'hello'"),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once()
            );
        }

        [Fact]
        public async Task PtyEndpoint_Handles_OpenOutputRequest()
        {
            // Given
            var mockEngine = _testHost.MockEngine;

            var terminal = new Terminal
            {
                Id = Guid.NewGuid().ToString(),
                Command = "bash",
            };

            var mockProcess = new Mock<IProcess>();
            var process = mockProcess.Object;

            var memory = new MemoryStream();
            var streamWriter = new StreamWriter(memory)
            {
                AutoFlush = true,
            };
            var streamReader = new StreamReader(memory);

            mockProcess.SetupSequence(proc => proc.IsRunning).Returns(true).Returns(false);
            mockProcess.SetupGet(proc => proc.Stdout).Returns(streamReader);
            mockEngine.Setup(engine => engine.TryGetProcess(terminal.Id, out process)).Returns(true);

            // When
            await streamWriter.WriteLineAsync("hello");
            memory.Seek(0, SeekOrigin.Begin);
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

            using var cts = new CancellationTokenSource(timeOut);
            var request = new OpenOutputRequest(terminal.Id);
            await socket.SendBinaryMessageAsync(writer, request, cts.Token);

            var response = await socket.ReceiveMessageAsync(reader, cts.Token);

            // Then
            var output = Assert.IsType<OutputEvent>(response);
            Assert.Equal(terminal.Id, output.TabId);
            var data = Encoding.UTF8.GetString(output.Data);
            Assert.Equal("hello", data.Trim('\r', '\n'));
        }
    }
}
