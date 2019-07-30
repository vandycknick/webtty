using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace WebTty.Messages.Events
{
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class StdErrorStream
    {
        [DefaultValue("v0")]
        [ReadOnly(true)]
        [Required]
        public string Version { get; }

        [Required]
        public string TabId { get; set; }

        public string Data { get; set; }
    }
}
