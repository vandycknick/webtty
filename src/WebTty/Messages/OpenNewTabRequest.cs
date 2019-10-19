using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

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
