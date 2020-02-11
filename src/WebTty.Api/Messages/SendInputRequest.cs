using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public struct SendInputRequest
    {
        [DataMember]
        public readonly string TabId;

        [DataMember]
        public readonly string Payload;

        public SendInputRequest(string tabId, string payload)
        {
            TabId = tabId;
            Payload = payload;
        }
    }
}
