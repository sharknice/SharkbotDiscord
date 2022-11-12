using Discord.WebSocket;
using Discord;

namespace SharkbotDiscord.Services.ImageGeneration
{
    public class GenerateImageResponseService
    {
        public GenerateImageResponseService()
        {
        }

        public async void GenerateImageResponse(SocketUserMessage e, string imagePath)
        {
            var filename = Path.GetFileName(imagePath);
            File.Copy(imagePath, filename);

            var emb = new EmbedBuilder()
                .WithImageUrl($"attachment://{filename}")
            .Build();

            await e.Channel.SendFileAsync(filename, null, false, emb);
        }
    }
}
