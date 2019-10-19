// using System;
// using System.Buffers;
// using WebTty.Messages;
// using WebTty.Messages.Commands;
// using WebTty.Serializers;
// using Xunit;

// namespace WebTty.Test.Serializers
// {
//     public class CommandDeserializersTests
//     {
//         [Fact]
//         public void CommandDeserializer_Deserialize_ShouldDeserializeOpenNewTabCommands()
//         {
//             // Given
//             var newTabCommand = new OpenNewTabCommand
//             {
//                 Title = "HelloWorld",
//             };
//             var message = new Message
//             {
//                 Type = nameof(OpenNewTabCommand),
//                 Payload = MessagePack.MessagePackSerializer.Serialize(newTabCommand),
//             };
//             var data = MessagePack.MessagePackSerializer.Serialize(message);

//             // When
//             var sequence = new ReadOnlySequence<byte>(data.AsMemory());
//             var deserialized = MessageDeserializer.Deserialize(sequence);

//             // Then
//             var command = Assert.IsType<OpenNewTabCommand>(deserialized);
//             Assert.Equal(expected: newTabCommand.Title, actual: command.Title);
//         }

//         [Fact]
//         public void CommandDeserializer_Deserialize_ShouldDeserializeResizeTabCommands()
//         {
//             // Given
//             var resizeTabCommand = new ResizeTabCommand
//             {
//                 TabId = Guid.NewGuid().ToString(),
//                 Cols = 100,
//                 Rows = 250,
//             };

//             var message = new Message
//             {
//                 Type = nameof(ResizeTabCommand),
//                 Payload = MessagePack.MessagePackSerializer.Serialize(resizeTabCommand),
//             };
//             var data = MessagePack.MessagePackSerializer.Serialize(message);

//             // When
//             var sequence = new ReadOnlySequence<byte>(data.AsMemory());
//             var deserialized = MessageDeserializer.Deserialize(sequence);

//             // Then
//             var command = Assert.IsType<ResizeTabCommand>(deserialized);
//             Assert.Equal(expected: resizeTabCommand.TabId, actual: command.TabId);
//             Assert.Equal(expected: resizeTabCommand.Cols, actual: command.Cols);
//             Assert.Equal(expected: resizeTabCommand.Rows, actual: command.Rows);
//         }

//         [Fact]
//         public void CommandDeserializer_Deserialize_ShouldDeserializeSendInputCommands()
//         {
//             // Given
//             var sendInputCommand = new SendInputCommand
//             {
//                 TabId = Guid.NewGuid().ToString(),
//                 Payload = "echo 'hello world'"
//             };

//             var message = new Message
//             {
//                 Type = nameof(SendInputCommand),
//                 Payload = MessagePack.MessagePackSerializer.Serialize(sendInputCommand),
//             };
//             var data = MessagePack.MessagePackSerializer.Serialize(message);

//             // When
//             var sequence = new ReadOnlySequence<byte>(data.AsMemory());
//             var deserialized = MessageDeserializer.Deserialize(sequence);

//             // Then
//             var command = Assert.IsType<SendInputCommand>(deserialized);
//             Assert.Equal(expected: sendInputCommand.TabId, actual: command.TabId);
//             Assert.Equal(expected: sendInputCommand.Payload, actual: command.Payload);
//         }

//         [MessagePack.MessagePackObject]
//         public class SomeUnknownCommand { }

//         [Fact]
//         public void CommandDeserializer_Deserialize_ShouldThrowAnUnknownCommandExceptionForUnknownPayloads()
//         {
//             // Given
//             var unknownCommand = new SomeUnknownCommand();
//             var message = new Message
//             {
//                 Type = nameof(SomeUnknownCommand),
//                 Payload = MessagePack.MessagePackSerializer.Serialize(unknownCommand),
//             };
//             var data = MessagePack.MessagePackSerializer.Serialize(message);

//             // When
//             var sequence = new ReadOnlySequence<byte>(data.AsMemory());

//             // Then
//             Assert.Throws<UnknownMessageException>(() =>
//             {
//                 var deserialized = MessageDeserializer.Deserialize(sequence);
//             });

//         }
//     }
// }
