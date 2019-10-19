using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebTty.Common;
using WebTty.Messages;

namespace WebTty.Protocol
{
    public class BinaryDeserializerMap
    {
        private readonly Dictionary<string, Type> _namedTypeMap = new Dictionary<string, Type>();
        private readonly Dictionary<Type, MessageDeserializerBase> _deserializerMap = new Dictionary<Type, MessageDeserializerBase>();

        public BinaryDeserializerMap()
        {
            var messageTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.IsClass && !string.IsNullOrEmpty(type.Namespace))
                .Where(type => {
                    var attributes = type.GetCustomAttributes(typeof(MessageAttribute), true);
                    return attributes != null && attributes.Length > 0;
                })
                .ToList();

            foreach (var type in messageTypes)
            {
                _namedTypeMap.Add(type.Name, type);
            }
        }

        public bool TryGetValue(string name, out MessageDeserializerBase deserializer)
        {
            if (_namedTypeMap.TryGetValue(name, out var type))
            {
                deserializer = _deserializerMap.GetOrAdd(
                    type,
                    () => (MessageDeserializerBase)Activator.CreateInstance(
                       typeof(MessageDeserializer<>).MakeGenericType(type))
                );
                return true;
            }

            deserializer = null;
            return false;
        }
    }
}
