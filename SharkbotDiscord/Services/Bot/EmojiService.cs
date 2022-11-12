using Discord;
using Discord.WebSocket;

namespace SharkbotDiscord.Services.Bot
{
    public class EmojiService
    {
        DiscordSocketClient discord;

        public EmojiService(DiscordSocketClient discordClient)
        {
            discord = discordClient;
        }

        public Emoji getEmoji(SocketUserMessage e, string chat)
        {
            Emoji.TryParse(chat, out var emoji);
            return emoji;
        }
    }
}
