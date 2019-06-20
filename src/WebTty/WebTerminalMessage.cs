using System;
using MessagePack;

namespace WebTty
{
    [MessagePackObject()]
    public struct WebTerminalOutputMessage
    {
        [Key(0)]
        public int Type { get; set; }

        [Key(1)]
        public int Id { get; set; }

        [Key(2)]
        public byte[] Body { get; set; }
    }

    [MessagePackObject()]
    public struct TerminalInputMessage
    {
        [Key(0)]
        public int Type { get; set; }

        [Key(1)]
        public int Id { get; set; }

        [Key(2)]
        public string Body { get; set; }
    }

    [MessagePackObject()]
    public struct TerminalResizeMessage
    {
        [Key(0)]
        public int Type { get => TerminalMessageTypes.TERMINAL_RESIZE; }

        [Key(1)]
        public int Id { get; set; }

        [Key(2)]
        public int Cols { get; set; }

        [Key(3)]
        public int Rows { get; set; }
    }

    [MessagePackObject()]
    public struct TerminalNewTabMessage
    {
        [Key(0)]
        public int Type { get => TerminalMessageTypes.TERMINAL_NEW_TAB; }
    }

    [MessagePackObject()]
    public struct TerminalNewTabCreatedMessage
    {
        [Key(0)]
        public int Type { get => TerminalMessageTypes.TERMINAL_NEW_TAB_CREATED; }

        [Key(1)]
        public int Id { get; set; }
    }

    public static class TerminalMessageTypes
    {
        public const int TERMINAL_INPUT = 1;
        public const int TERMINAL_RESIZE = 3;
        public const int TERMINAL_NEW_TAB = 4;
        public const int TERMINAL_NEW_TAB_CREATED = 5;
    }
}
