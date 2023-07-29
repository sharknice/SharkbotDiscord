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
            var emb = new EmbedBuilder()
                .WithImageUrl($"attachment://{imagePath}")
            .Build();

            await e.Channel.SendFileAsync(imagePath, null, false, emb, messageReference: new MessageReference(e.Id));
        }
    }
}
