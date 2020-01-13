using System.Runtime.Serialization;

namespace WebTty.Application.Requests
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
