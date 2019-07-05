using MessagePack;

namespace WebTty.Messages
{
    [MessagePackObject]
    public struct TerminalInput
    {
        [Key(0)]
        public int Type => (int)TerminalMessageTypes.TerminalInput;

        [Key(1)]
        public int Id { get; set; }

        [Key(2)]
        public string Body { get; set; }
    }
}
