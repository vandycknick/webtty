using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public struct OpenNewTabRequest
    {
        [DataMember]
        public readonly string Title;

        public OpenNewTabRequest(string title)
        {
            Title = title;
        }
    }

    [DataContract]
    public struct OpenNewTabReply
    {
        [DataMember]
        public readonly string Id;

        public OpenNewTabReply(string id)
        {
            Id = id;
        }
    }
}
