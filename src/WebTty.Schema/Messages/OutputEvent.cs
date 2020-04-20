using System;
using System.Runtime.Serialization;

namespace WebTty.Schema.Messages
{

    [DataContract]
    public readonly struct OutputEvent
    {
        [DataMember]
        public string TabId { get; }

        [DataMember]
        public ArraySegment<byte> Data { get; }

        public OutputEvent(string tabId, ArraySegment<byte> data)
        {
            TabId = tabId;
            Data = data;
        }
    }
}
