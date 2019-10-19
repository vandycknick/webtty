using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebTty.Messages
{
    [Message]
    [Serializable]
    public class StdOutMessage : INotification
    {
        [Required]
        public string TabId { get; set; }

        [Required]
        public ArraySegment<byte> Data { get; set; }
    }
}
