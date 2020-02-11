using System.Reflection;

namespace WebTty.Api.Infrastructure
{
    public enum MessageFormat
    {
        Binary,
        Json
    }

    public class MessagingOptions
    {
        public Assembly MessageSource { get; set; }
        public MessageFormat Format { get; set; } = MessageFormat.Binary;
    }
}
