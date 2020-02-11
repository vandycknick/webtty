namespace WebTty.Api.Messages
{
    public struct UnknownMessageEvent
    {
        public readonly string Name;

        public UnknownMessageEvent(string name)
        {
            Name = name;
        }
    }
}
