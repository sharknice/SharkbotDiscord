using Discord.WebSocket;

namespace SharkbotDiscord.Services.ImageGeneration
{
    public class ImageResponseUtility
    {
        public ImageResponseUtility()
        {

        }

        public bool AskingForImageResponse(SocketUserMessage e)
        {
            if (e.Content.ToLower() == "draw me")
            {
                return true;
            }
            return false;
        }
    }
}
