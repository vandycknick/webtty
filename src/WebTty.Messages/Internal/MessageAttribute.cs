using System;
using MessagePack;

namespace WebTty.Messages.Internal
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class MessageAttribute : MessagePackObjectAttribute
    {
        public MessageAttribute() : base(true)
        {
        }
    }
}
