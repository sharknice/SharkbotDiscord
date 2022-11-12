using Discord;
using Discord.WebSocket;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.Bot
{
    public class RequiredPropertyResponseService
    {
        IDiscordClient discord;
        BotUtilityService utilityService;
        EmojiService emojiService;

        public RequiredPropertyResponseService(IDiscordClient discordClient, BotUtilityService botUtilityService, EmojiService botEmojiService)
        {
            discord = discordClient;
            utilityService = botUtilityService;
            emojiService = botEmojiService;
        }

        public async void hasRequiredPropertyResponse(SocketUserMessage e, ChatResponse chatResponse, ChannelSettings channelSettings)
        {
            if (utilityService.alwaysRespond(e) || chatResponse.confidence > channelSettings.TargetedResponseConfidenceThreshold)
            {
                var typeTime = 0;
                foreach (var chat in chatResponse.response)
                {
                    Emoji emoji = null;
                    try
                    {
                        emoji = emojiService.getEmoji(e, chat.Trim());
                    }
                    catch (Exception)
                    {

                    }

                    if (emoji != null)
                    {
                        await e.AddReactionAsync(emoji);
                    }
                    else
                    {
                        await Task.Delay(typeTime).ContinueWith((task) => { e.Channel.TriggerTypingAsync(); });

                        var response = chat.Trim();

                        response = utilityService.formatResponse(e, chat);

                        typeTime += utilityService.getTypeTime(chat);
                        await Task.Delay(typeTime).ContinueWith((task) => { e.ReplyAsync(response); });
                    }
                }
            }
        }
    }
}
