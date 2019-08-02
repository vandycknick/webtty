using System;
using System.ComponentModel.DataAnnotations;

namespace WebTty.Messages
{
    [MessagePack.MessagePackObject]
    [Serializable]
    public class Message
    {
        [Required]
        [MessagePack.Key(0)]
        public string Type { get; set; }

        [Required]
        [MessagePack.Key(1)]
        public ArraySegment<byte> Payload { get; set; }
    }
}
