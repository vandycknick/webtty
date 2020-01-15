using System;
using System.Collections.Generic;

namespace WebTty.Infrastructure.Protocol
{
    public interface IMessageResolver
    {
        IReadOnlyCollection<Type> GetMessages();
        bool TryGetMessageType(string name, out Type type);
        bool TryGetMessageId(Type type, out string name);
    }
}
