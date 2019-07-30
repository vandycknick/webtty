using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace WebTty.Messages.Events
{
    [MessagePackObject(keyAsPropertyName: true)]
    [Serializable]
    public class TabOpened
    {
        [DefaultValue("v0")]
        [ReadOnly(true)]
        [Required]
        public string Version { get; }

        [Required]
        public string Id { get; set; }
    }
}
