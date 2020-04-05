using System.Runtime.Serialization;

namespace WebTty.Schema.Messages
{
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
