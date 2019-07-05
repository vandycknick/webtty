using MessagePack;

namespace WebTty.Messages
{
    [MessagePackObject]
    public struct NewSessionResponse
    {
        [Key(0)]
        public int Type => (int)TerminalMessageTypes.NewSessionResponse;

        [Key(1)]
        public int Id { get; set; }
    }
}
