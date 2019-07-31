using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace WebTty.Messages.Commands
{
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class ResizeTabCommand
    {
        [DefaultValue("v0")]
        [ReadOnly(true)]
        public string Version { get; }

        [Required]
        public string TabId { get; set; }

        [DefaultValue(80)]
        public int Cols { get; set; }

        [DefaultValue(24)]
        public int Rows { get; set; }
    }
}
