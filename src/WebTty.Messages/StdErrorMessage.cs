using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using WebTty.Messages.Internal;

namespace WebTty.Messages
{
    [Message]
    [Serializable]
    public class StdErrorMessage : INotification
    {
        [Required]
        public string TabId { get; set; }

        [Required]
        public ArraySegment<byte> Data { get; set; }
    }
}
