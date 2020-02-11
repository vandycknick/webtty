using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public struct OpenOutputRequest
    {
        [DataMember]
        public readonly string TabId;

        public OpenOutputRequest(string tabId)
        {
            TabId = tabId;
        }
    }
}
