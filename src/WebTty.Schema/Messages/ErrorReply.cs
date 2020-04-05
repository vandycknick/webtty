using System.Runtime.Serialization;

namespace WebTty.Schema.Messages
{
    [DataContract]
    public readonly struct ErrorReply
    {
        [DataMember]
        public string Id { get; }

        [DataMember]
        public string ParentId { get; }

        [DataMember]
        public string Message { get; }

        public ErrorReply(string id, string parentId, string message)
        {
            Id = id;
            ParentId = parentId;
            Message = message;
        }
    }
}
