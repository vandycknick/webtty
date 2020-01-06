using MediatR;
using System;
using System.Runtime.Serialization;

namespace WebTty.Messages
{
    [DataContract]
    public class StdOutMessage : INotification
    {
        [DataMember]
        public readonly string TabId;

        [DataMember]
        public readonly ArraySegment<byte> Data;

        public StdOutMessage(string tabId, ArraySegment<byte> data)
        {
            TabId = tabId;
            Data = data;
        }
    }
}
