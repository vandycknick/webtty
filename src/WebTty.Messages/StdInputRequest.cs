using MediatR;
using System.Runtime.Serialization;

namespace WebTty.Messages
{
    [DataContract]
    public class StdInputRequest : IRequest
    {
        [DataMember]
        public readonly string TabId;

        [DataMember]
        public readonly string Payload;

        public StdInputRequest(string tabId, string payload)
        {
            TabId = tabId;
            Payload = payload;
        }
    }
}
