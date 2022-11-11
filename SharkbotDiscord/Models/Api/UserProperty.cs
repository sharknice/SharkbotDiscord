namespace SharkbotDiscord.Models.Api
{
    [Serializable]
    public class UserProperty
    {
        public UserProperty()
        {
            name = string.Empty;
            value = string.Empty;
            source = string.Empty;
            time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public string name { get; set; }
        public string value { get; set; }
        public string source { get; set; }
        public long time { get; set; }
    }
}
