using System;
using System.Collections.Generic;
using System.Linq;

namespace WebTty.Api.Infrastructure.Core
{
    internal static class ReflectionHelpers
    {
        public static bool IsIAsyncEnumerable(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
            }

            return type.GetInterfaces().Any(t =>
            {
                if (t.IsGenericType)
                {
                    return t.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
                }
                else
                {
                    return false;
                }
            });
        }
    }
}
