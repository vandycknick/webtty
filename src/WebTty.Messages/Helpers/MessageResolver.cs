using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebTty.Messages.Internal;

namespace WebTty.Messages.Helpers
{
    public class MessageResolver : IMessageResolver
    {
        public IReadOnlyCollection<Type> GetMessages()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types =
                from t in assembly.GetTypes()
                where t.IsClass && !string.IsNullOrEmpty(t.Namespace)
                let attributes = t.GetCustomAttributes(typeof(MessageAttribute), true)
                where attributes != null && attributes.Length > 0
                select t;

            return types.ToList();
        }
    }
}
