namespace SharkbotDiscord.Services.ImageGeneration
{
    public class GenerationData
    {
        public GenerationData(string userName, string text)
        {
            UserName = userName;
            Text = text;
        }

        public string UserName { get; set; }
        public string Text { get; set; }
    }
}
