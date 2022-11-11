namespace SharkbotDiscord.Models.Api
{
    [Serializable]
    public class ChatResponse
    {
        public List<string> response { get; set; }
        public double confidence { get; set; }
        public dynamic metadata { get; set; }
    }
}
