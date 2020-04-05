using System.Runtime.Serialization;

namespace WebTty.Schema.Messages
{
    [DataContract]
    public readonly struct ResizeTabRequest
    {
        [DataMember]
        public string TabId { get; }

        [DataMember]
        public int Cols { get; }

        [DataMember]
        public int Rows { get; }

        public ResizeTabRequest(string tabId, int cols = 80, int rows = 24)
        {
            TabId = tabId;
            Cols = cols;
            Rows = rows;
        }
    }
}
