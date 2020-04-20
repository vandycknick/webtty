using System.Runtime.Serialization;

namespace WebTty.Schema.Messages
{
    [DataContract]
    public readonly struct OpenOutputRequest
    {
        [DataMember]
        public string TabId { get; }

        public OpenOutputRequest(string tabId)
        {
            TabId = tabId;
        }
    }
}
