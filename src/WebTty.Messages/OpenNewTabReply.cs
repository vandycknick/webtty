using System;
using System.ComponentModel.DataAnnotations;
using WebTty.Messages.Internal;

namespace WebTty.Messages
{
    [Message]
    [Serializable]
    public class OpenNewTabReply
    {
        [Required]
        public string Id { get; set; }
    }
}
