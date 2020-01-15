namespace WebTty.Integration.Test
{
    public class ResizeWindowTests
    {
        // private const int TimeOutMilliseconds = 5000;

        // [Fact]
        // public async Task WebTty_ResizeWindow_UpdatesWindowSize()
        // {
        //     // Arrange
        //     var uri = new Uri("ws://localhost:5000/ws");
        //     NewSessionResponse tab;
        //     var resizeResponse = "";

        //     // Act
        //     using (var socket = new ClientWebSocket())
        //     {
        //         using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
        //         {
        //             var task = socket.ConnectAsync(uri, cts.Token);
        //             Assert.True(
        //                 (socket.State == WebSocketState.None) ||
        //                 (socket.State == WebSocketState.Connecting) ||
        //                 (socket.State == WebSocketState.Open),
        //                 "State immediately after ConnectAsync incorrect: " + socket.State);
        //             await task;
        //         }

        //         Assert.Equal(WebSocketState.Open, socket.State);

        //         var newSessionMsg = new NewSessionRequest();
        //         var data = MessagePack.MessagePackSerializer.Serialize(newSessionMsg);

        //         using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
        //         {
        //             await socket.SendAsync(data, WebSocketMessageType.Binary, true, cts.Token);
        //         }

        //         var buffer = new byte[65536];
        //         var segment = new ArraySegment<byte>(buffer, 0, buffer.Length);

        //         using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
        //         {
        //             await socket.ReceiveAsync(segment, cts.Token);
        //             tab = MessagePack.MessagePackSerializer.Deserialize<NewSessionResponse>(segment);
        //         }

        //         var resizeMsg = new TerminalResize
        //         {
        //             Id = tab.Id,
        //             Cols = 100,
        //             Rows = 50,
        //         };
        //         data = MessagePack.MessagePackSerializer.Serialize(resizeMsg);

        //         using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
        //         {
        //             await socket.SendAsync(data, WebSocketMessageType.Binary, true, cts.Token);
        //         }

        //         var inputMsg = new TerminalInput
        //         {
        //             Id = tab.Id,
        //             Body = "echo \"<<< $(stty size) >>>\"\n",
        //         };
        //         data = MessagePack.MessagePackSerializer.Serialize(inputMsg);

        //         using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
        //         {
        //             await socket.SendAsync(data, WebSocketMessageType.Binary, true, cts.Token);
        //         }

        //         resizeResponse = "";

        //         try
        //         {
        //             var keepGoing = true;
        //             while (keepGoing)
        //             {
        //                 using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
        //                 {
        //                     await socket.ReceiveAsync(segment, cts.Token);
        //                     var response = MessagePack.MessagePackSerializer.Deserialize<TerminalOutput>(segment);
        //                     var body = Encoding.UTF8.GetString(response.Body);
        //                     resizeResponse = body;

        //                     if (body.StartsWith("<<<") && body.EndsWith(">>>\r\n"))
        //                     {
        //                         keepGoing = false;
        //                     }
        //                 }
        //             }
        //         }
        //         catch (TaskCanceledException)
        //         {
        //         }

        //         await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        //     }

        //     // Assert
        //     Assert.Contains(expectedSubstring: "50 100", actualString: resizeResponse);
        // }
    }
}
