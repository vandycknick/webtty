using MediatR;
using System;
using System.Runtime.Serialization;

namespace WebTty.Messages
{
    [DataContract]
    public class StdErrorMessage : INotification
    {
        [DataMember]
        public readonly string TabId;

        [DataMember]
        public readonly ArraySegment<byte> Data;

        public StdErrorMessage(string tabId, ArraySegment<byte> data)
        {
            TabId = tabId;
            Data = data;
        }
    }
}
