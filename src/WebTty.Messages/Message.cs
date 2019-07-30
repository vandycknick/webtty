using System;
using MessagePack;

namespace WebTty.Messages
{
    [MessagePackObject]
    [Serializable]
    public class Message
    {
        [Key(0)]
        public string Type { get; set; }

        [Key(1)]
        public byte[] Payload { get; set; }
    }
}
