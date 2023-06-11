namespace SharkbotDiscord.Services.MusicGeneration
{
    public class MusicApiResponse
    {
        public List<object> data { get; set; }
        public bool is_generating { get; set; }
        public double duration { get; set; }
        public double average_duration { get; set; }
    }

    public class MusicApiResponseData
    {
        public string name { get; set; }
        public object data { get; set; }
        public bool is_file { get; set; }
    }
}
