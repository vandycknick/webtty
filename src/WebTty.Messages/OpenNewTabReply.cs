using System.Runtime.Serialization;

namespace WebTty.Messages
{
    [DataContract]
    public class OpenNewTabReply
    {
        [DataMember]
        public readonly string Id;

        public OpenNewTabReply(string id)
        {
            Id = id;
        }
    }
}
