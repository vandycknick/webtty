using System;
using MessagePack;

namespace WebTty.Messages.Internal
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class MessageAttribute : MessagePackObjectAttribute
    {
        public MessageAttribute() : base(true)
        {
        }
    }
}
