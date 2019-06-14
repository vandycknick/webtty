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

    [MessagePackObject()]
    public struct TerminalInputMessage
    {
        [Key(0)]
        public int Type { get; set; }

        [Key(1)]
        public string Body { get; set; }
    }

    [MessagePackObject()]
    public struct TerminalResizeMessage
    {
        [Key(0)]
        public int Type { get; set; }

        [Key(1)]
        public int Cols { get; set; }

        [Key(2)]
        public int Rows { get; set; }
    }
}
