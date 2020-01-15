using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using WebTty.Infrastructure.Protocol;

namespace WebTty.Application.Common
{
    public class MessageResolver : IMessageResolver
    {
        private readonly Dictionary<string, Type> _names = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> _types;

        public MessageResolver()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types =
                from t in assembly.GetTypes()
                where t.IsClass || t.IsValueType
                where !t.IsPrimitive && !string.IsNullOrEmpty(t.Namespace)
                let attributes = t.GetCustomAttributes(typeof(DataContractAttribute), true)
                where attributes != null && attributes.Length > 0
                select t;

            foreach (var type in types)
            {
                _names.Add(type.Name, type);
            }

            _types = _names.ToDictionary(x => x.Value, x=> x.Key);
        }

        public IReadOnlyCollection<Type> GetMessages() =>
            _types.Keys.ToList();

        public bool TryGetMessageId(Type type, out string name) =>
            _types.TryGetValue(type, out name);

        public bool TryGetMessageType(string name, out Type type) =>
            _names.TryGetValue(name, out type);
    }
}
