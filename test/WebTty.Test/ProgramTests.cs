// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Hosting;
// using Moq;
// using Serilog;
// using WebTty.Hosting.Models;
// using Xunit;

// namespace WebTty.Test
// {
//     public class ProgramTests
//     {
//         private Mock<IHostBuilder> MockedHostBuilder { get; set; }
//         private Mock<IHost> MockedHost { get; set; }
//         private Mock<ILogger> MockedLogger { get; set; }

//         public ProgramTests()
//         {
//             MockedHostBuilder = new Mock<IHostBuilder>();
//             MockedHost = new Mock<IHost>();
//             MockedHostBuilder.Setup(b => b.Build()).Returns(MockedHost.Object);

//             MockedLogger = new Mock<ILogger>();
//         }

//         [Fact]
//         public async Task Program_RunAsync_PrintsAMesssageWhenTheServerIsUpAndRunning()
//         {
//             // Given
//             var options = new Settings();

//             // When
//             var program = new Program(MockedHostBuilder.Object, options, MockedLogger.Object);
//             await program.RunAsync(new CancellationToken(true));

//             // Then
//             MockedLogger.Verify(logger =>
//                 logger.Information("Listening on: http://{address}:{port}", options.Address, options.Port));

//             MockedLogger.Verify(logger =>
//                 logger.Information("Press CTRL+C to exit"));
//         }

//         [Fact]
//         public async Task Program_RunAsync_WritesAnErrorMessageWhenTheServerThrowsAnExceptionOnStartup()
//         {
//             // Given
//             var options = new Settings();
//             MockedHost
//                 .Setup(host => host.StartAsync(It.IsAny<CancellationToken>()))
//                 .ThrowsAsync(new Exception("some error"));

//             // When
//             var program = new Program(MockedHostBuilder.Object, options, MockedLogger.Object);
//             var result = await program.RunAsync(new CancellationToken(true));

//             // Then
//             MockedLogger.Verify(logger =>
//                 logger.Fatal(It.IsAny<Exception>(), "Host terminated unexpectedly"));
//         }

//         [Fact]
//         public async Task Program_RunAsync_ReturnsOneWhenTheServerThrowsAnExceptionOnStartup()
//         {
//             // Given
//             var options = new Settings();
//             MockedHost
//                 .Setup(host => host.StartAsync(It.IsAny<CancellationToken>()))
//                 .ThrowsAsync(new Exception("some error"));

//             // When
//             var program = new Program(MockedHostBuilder.Object, options, MockedLogger.Object);
//             var result = await program.RunAsync(new CancellationToken(true));

//             // Then
//             Assert.Equal(1, result);
//         }

//         [Fact]
//         public async Task Program_RunAsync_WritesAnErrorMessageWhenTheServerThrowsAnExceptionWhileShuttingDown()
//         {
//             // Given
//             var options = new Settings();
//             MockedHost
//                 .Setup(host => host.StopAsync(It.IsAny<CancellationToken>()))
//                 .ThrowsAsync(new Exception("some error"));


//             // When
//             var program = new Program(MockedHostBuilder.Object, options, MockedLogger.Object);
//             var result = await program.RunAsync(new CancellationToken(true));

//             // Then
//             MockedLogger.Verify(logger =>
//                 logger.Fatal(It.IsAny<Exception>(), "Host terminated unexpectedly"),
//                 Times.Once());
//         }

//         [Fact]
//         public async Task Program_RunAsync_ReturnsOneWhenTheServerThrowsAnExceptionWhileShuttingDown()
//         {
//             // Given
//             var options = new Settings();
//             MockedHost
//                 .Setup(host => host.StopAsync(It.IsAny<CancellationToken>()))
//                 .ThrowsAsync(new Exception("some error"));

//             // When
//             var program = new Program(MockedHostBuilder.Object, options, MockedLogger.Object);
//             var result = await program.RunAsync(new CancellationToken(true));

//             // Then
//             Assert.Equal(1, result);
//         }
//     }
// }
