using Discord.WebSocket;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.Bot
{
    public class BotReactionService
    {
        BotConfiguration configuration;
        EmojiService emojiService;

        public BotReactionService(BotConfiguration botConfiguration, EmojiService botEmojiService)
        {
            configuration = botConfiguration;
            emojiService = botEmojiService;
        }

        public async void reactionResponse(SocketUserMessage e, ChatResponse chatResponse, ChannelSettings channelSettings)
        {
            if (chatResponse.confidence > channelSettings.ReactionConfidenceThreshold)
            {
                var reactions = 0;
                foreach (var chat in chatResponse.response)
                {
                    var emoji = emojiService.getEmoji(e, chat.Trim());

                    if (emoji != null)
                    {
                        try
                        {
                            await e.AddReactionAsync(emoji);
                        }
                        catch (Exception)
                        {

                        }

                        reactions++;
                        if (reactions >= configuration.MaximumReactionsPerMessage)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
