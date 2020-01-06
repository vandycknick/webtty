using MediatR;
using System.Runtime.Serialization;

namespace WebTty.Messages
{
    [DataContract]
    public class ResizeTabMessage : INotification
    {
        [DataMember]
        public readonly string TabId;

        [DataMember]
        public readonly int Cols;

        [DataMember]
        public readonly int Rows;

        public ResizeTabMessage(string tabId, int cols = 80, int rows = 24)
        {
            TabId = tabId;
            Cols = cols;
            Rows = rows;
        }
    }
}
