using System;
using System.ComponentModel.DataAnnotations;

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
