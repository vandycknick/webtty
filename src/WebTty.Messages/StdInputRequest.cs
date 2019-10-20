using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using WebTty.Messages.Internal;

namespace WebTty.Messages
{
    [Message]
    [Serializable]
    public class StdInputRequest : IRequest
    {
        [Required]
        public string TabId { get; set; }

        public string Payload { get; set; }
    }
}
