using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace WebTty.Messages.Commands
{
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class OpenNewTabCommand
    {
        [DefaultValue("v0")]
        [ReadOnly(true)]
        [Required]
        public string Version { get; }

        [Required]
        public string Title { get; set; }
    }
}
