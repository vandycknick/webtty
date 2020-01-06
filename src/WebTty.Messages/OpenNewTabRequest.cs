using MediatR;
using System.Runtime.Serialization;

namespace WebTty.Messages
{
    [DataContract]
    public class OpenNewTabRequest : IRequest<OpenNewTabReply>
    {
        [DataMember]
        public readonly string Title;

        public OpenNewTabRequest(string title)
        {
            Title = title;
        }
    }
}
