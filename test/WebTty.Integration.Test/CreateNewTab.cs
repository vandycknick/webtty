// using System;
// using System.Net.WebSockets;
// using System.Threading;
// using System.Threading.Tasks;
// using WebTty.Messages;
// using WebTty.Messages.Commands;
// using Xunit;

// namespace WebTty.Integration.Test
// {
//     public class CreateNewTab
//     {
//         // private const int TimeOutMilliseconds = 5000;

//         // [Fact]
//         // public async Task WebTty_NewTabRequest_ShouldCreateANewSession()
//         // {
//             // // Arrange
//             // var uri = new Uri("ws://localhost:5000/ws");
//             // WebSocketReceiveResult result;
//             // OpenNewTabCommand newTab;

//             // // Act
//             // using (var socket = new ClientWebSocket())
//             // {
//             //     using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
//             //     {
//             //         var task = socket.ConnectAsync(uri, cts.Token);
//             //         Assert.True(
//             //             (socket.State == WebSocketState.None) ||
//             //             (socket.State == WebSocketState.Connecting) ||
//             //             (socket.State == WebSocketState.Open),
//             //             "State immediately after ConnectAsync incorrect: " + socket.State);
//             //         await task;
//             //     }

//             //     Assert.Equal(WebSocketState.Open, socket.State);

//             //     var msg = new OpenNewTabCommand();
//             //     var data = MessagePack.MessagePackSerializer.Serialize(msg);

//             //     using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
//             //     {
//             //         await socket.SendAsync(data, WebSocketMessageType.Binary, true, cts.Token);
//             //     }

//             //     byte[] buffer = new byte[65536];
//             //     var segment = new ArraySegment<byte>(buffer, 0, buffer.Length);

//             //     using (var cts = new CancellationTokenSource(TimeOutMilliseconds))
//             //     {
//             //         result = await socket.ReceiveAsync(segment, cts.Token);
//             //     }

//             //     var message = MessagePack.MessagePackSerializer.Deserialize<Message>(segment);

//             //     await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
//             // }

//             // // Assert
//             // Assert.Equal(WebSocketMessageType.Binary, result.MessageType);
//             // Assert.InRange(actual: newTab.Id, low: 0, high: 1000);
//         // }
//     }
// }
