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
            Directory.CreateDirectory("images");
            File.Copy(imagePath, "images/" + filename);

            var emb = new EmbedBuilder()
                .WithImageUrl($"attachment://{filename}")
            .Build();

            await e.Channel.SendFileAsync("images/" + filename, null, false, emb, messageReference: new MessageReference(e.Id));
        }
    }
}
