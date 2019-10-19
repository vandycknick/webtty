using System;
using MessagePack;

namespace WebTty.Messages
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MessageAttribute : MessagePackObjectAttribute
    {
        public MessageAttribute() : base(true)
        {
        }
    }
}
