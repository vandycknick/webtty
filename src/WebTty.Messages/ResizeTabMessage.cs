using MediatR;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebTty.Messages.Internal;

namespace WebTty.Messages
{
    [Message]
    [Serializable]
    public class ResizeTabMessage : INotification
    {
        [Required]
        public string TabId { get; set; }

        [DefaultValue(80)]
        public int Cols { get; set; }

        [DefaultValue(24)]
        public int Rows { get; set; }
    }
}
