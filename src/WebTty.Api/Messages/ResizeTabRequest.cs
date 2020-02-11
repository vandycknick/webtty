using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public struct ResizeTabRequest
    {
        [DataMember]
        public readonly string TabId;

        [DataMember]
        public readonly int Cols;

        [DataMember]
        public readonly int Rows;

        public ResizeTabRequest(string tabId, int cols = 80, int rows = 24)
        {
            TabId = tabId;
            Cols = cols;
            Rows = rows;
        }
    }
}
