using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace WebTty.Messages.Events
{
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class StdOutStream
    {
        [DefaultValue("v0")]
        [ReadOnly(true)]
        [Required]
        public string Version { get; }

        [Required]
        public string TabId { get; set; }

        [Required]
        public ArraySegment<byte> Data { get; set; }
    }
}
