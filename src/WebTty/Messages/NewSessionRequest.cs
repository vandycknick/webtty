using MessagePack;

namespace WebTty.Messages
{
    [MessagePackObject]
    public struct NewSessionRequest
    {
        [Key(0)]
        public int Type => (int)TerminalMessageTypes.NewSessionRequest;
    }
}
