using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using WebTty.Messages.Internal;

namespace WebTty.Messages
{
    [Message]
    [Serializable]
    public class OpenNewTabRequest : IRequest<OpenNewTabReply>
    {
        [Required]
        public string Title { get; set; }
    }
}
