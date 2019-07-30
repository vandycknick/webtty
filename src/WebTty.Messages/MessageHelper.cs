using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WebTty.Messages
{
    public static class MessageHelper
    {
        public static Type[] ListAllEventsAndCommands()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !string.IsNullOrEmpty(t.Namespace) &&
                    t.GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0 &&
                    t.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0
                )
                .ToArray();
        }
    }
}
