using System.Runtime.Serialization;

namespace WebTty.Schema
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
}
