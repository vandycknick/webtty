namespace WebTty.Schema
{
    public readonly struct UnknownMessageEvent
    {
        public string Name { get; }

        public UnknownMessageEvent(string name)
        {
            Name = name;
        }
    }
}
