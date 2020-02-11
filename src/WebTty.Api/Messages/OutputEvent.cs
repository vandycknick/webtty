using System;
using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{

    [DataContract]
    public struct OutputEvent
    {
        [DataMember]
        public readonly string TabId;

        [DataMember]
        public readonly ArraySegment<byte> Data;

        public OutputEvent(string tabId, ArraySegment<byte> data)
        {
            TabId = tabId;
            Data = data;
        }
    }
}
