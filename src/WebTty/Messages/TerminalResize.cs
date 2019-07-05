using MessagePack;

namespace WebTty.Messages
{
    [MessagePackObject]
    public struct TerminalResize
    {
        [Key(0)]
        public int Type => (int)TerminalMessageTypes.TerminalResizeRequest;

        [Key(1)]
        public int Id { get; set; }

        [Key(2)]
        public int Cols { get; set; }

        [Key(3)]
        public int Rows { get; set; }
    }
}
