using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public struct OpenNewTabRequest
    {
        [DataMember]
        public readonly string Id;

        [DataMember]
        public readonly string Title;

        public OpenNewTabRequest(string id, string title)
        {
            Id = id;
            Title = title;
        }
    }

    [DataContract]
    public struct OpenNewTabReply
    {
        [DataMember]
        public readonly string Id;

        [DataMember]
        public readonly string ParentId;

        public OpenNewTabReply(string id, string parentId)
        {
            Id = id;
            ParentId = parentId;
        }
    }
}
