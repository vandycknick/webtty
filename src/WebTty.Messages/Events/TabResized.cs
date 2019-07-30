using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace WebTty.Messages.Events
{
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class TabResized
    {
        [DefaultValue("v0")]
        [ReadOnly(true)]
        [Required]
        public string Version { get; }

        [Required]
        public string Id { get; set; }

        public int Cols { get; set; }

        public int Rows { get; set; }
    }
}
