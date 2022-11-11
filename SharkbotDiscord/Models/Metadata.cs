using System;

namespace SharkbotDiscord.Services.Models
{
    [Serializable]
    public class Metadata
    {
        public ulong guildId { get; set; }
        public ulong channelId { get; set; }
    }
}
