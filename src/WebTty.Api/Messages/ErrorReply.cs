using System.Runtime.Serialization;

namespace WebTty.Api.Messages
{
    [DataContract]
    public struct ErrorReply
    {
        [DataMember]
        public readonly string Id;

        [DataMember]
        public readonly string ParentId;

        [DataMember]
        public readonly string Message;

        public ErrorReply(string id, string parentId, string message)
        {
            Id = id;
            ParentId = parentId;
            Message = message;
        }
    }
}
