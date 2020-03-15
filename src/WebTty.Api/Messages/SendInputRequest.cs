using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public readonly struct SendInputRequest
    {
        [DataMember]
        public string TabId { get; }

        [DataMember]
        public string Payload { get; }

        public SendInputRequest(string tabId, string payload)
        {
            TabId = tabId;
            Payload = payload;
        }
    }
}
