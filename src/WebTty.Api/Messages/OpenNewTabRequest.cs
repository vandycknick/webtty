using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public readonly struct OpenNewTabRequest
    {
        [DataMember]
        public string Id { get; }

        [DataMember]
        public string Title { get; }

        public OpenNewTabRequest(string id, string title)
        {
            Id = id;
            Title = title;
        }
    }

    [DataContract]
    public readonly struct OpenNewTabReply
    {
        [DataMember]
        public string Id { get; }

        [DataMember]
        public string ParentId { get; }

        public OpenNewTabReply(string id, string parentId)
        {
            Id = id;
            ParentId = parentId;
        }
    }
}
