namespace WebTty.Messages
{
    public enum TerminalMessageTypes : int
    {
        TerminalInput = 1,
        TerminalOutput = 2,
        TerminalResizeRequest = 3,
        NewSessionRequest = 4,
        NewSessionResponse = 5,
    }
}
