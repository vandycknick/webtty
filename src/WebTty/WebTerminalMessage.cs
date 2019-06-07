using System;
using MessagePack;

namespace WebTty
{
    [MessagePackObject()]
    public struct WebTerminalMessage
    {
        [Key(0)]
        public int Type { get; set; }

        [Key(1)]
        public byte[] Body { get; set; }
    }
}
