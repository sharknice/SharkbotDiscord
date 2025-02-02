namespace SharkbotDiscord.Services.Models
{
    public class BotConfiguration
    {
        public string Token { get; set; }
        public string ApiUrl { get; set; }
        public string OllamaApiUrl { get; set; }
        public string ImageApiUrl { get; set; }
        public string MusicApiUrl { get; set; }

        public string OllamaModel { get; set; }
        public string OllamaSystemPrompt { get; set; }
        public double OllamaConfidence { get; set; }
        public double OllamaChance { get; set; }
        public double OllamaReplyChance { get; set; }
        public string ImageApiDirectory { get; set; }
        public string BotName { get; set; }
        public List<string> IgnoredChannels { get; set; }
        public string ChatType { get; set; }
        public List<string> ExclusiveTypes { get; set; }
        public List<string> RequiredProperyMatches { get; set; }
        public List<string> NickNames { get; set; }
        public double TargetedResponseConfidenceThreshold { get; set; }
        public double ReactionConfidenceThreshold { get; set; }
        public int MaximumReactionsPerMessage { get; set; }
        public string DefaultResponse { get; set; }
        public string MongoDbConnectionString { get; set; }

        public List<Channel> Channels { get; set; }
    }
}
