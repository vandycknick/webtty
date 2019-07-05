using MessagePack;

namespace WebTty.Messages
{
    [MessagePackObject]
    public class TerminalOutput
    {
        [Key(0)]
        public int Type => (int)TerminalMessageTypes.TerminalOutput;

        [Key(1)]
        public int Id { get; set; }

        [Key(2)]
        public byte[] Body { get; set; }
    }
}
