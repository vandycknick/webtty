using System;
using System.Collections.Generic;

namespace WebTty.Messages.Helpers
{
    public interface IMessageResolver
    {
        IReadOnlyCollection<Type> GetMessages();
    }
}
