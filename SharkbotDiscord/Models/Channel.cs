using MongoDB.Bson;

namespace SharkbotDiscord.Services.Models
{
    [Serializable]
    public class Channel
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public ulong ChannelId { get; set; }
        public ChannelSettings ChannelSettings { get; set; }
    }
}
