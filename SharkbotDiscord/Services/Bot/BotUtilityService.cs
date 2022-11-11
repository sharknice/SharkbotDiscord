using Discord;
using Discord.WebSocket;
using SharkbotDiscord.Services.Models;
using System.Text.RegularExpressions;

namespace SharkbotDiscord.Services.Bot
{
    public class BotUtilityService
    {
        DiscordSocketClient discord;
        BotConfiguration configuration;

        public BotUtilityService(DiscordSocketClient discordClient, BotConfiguration botConfiguration)
        {
            discord = discordClient;
            configuration = botConfiguration;
        }

        public string formatResponse(SocketUserMessage e, string chat)
        {
            var response = chat.Trim();

            if (response.StartsWith("/me "))
            {
                response = $"*{response}*";
            }

            var regex = new Regex("@(?<name>[^\\s]+)");
            var results = regex.Matches(response)
                .Cast<Match>()
                .Select(m => m.Groups["name"].Value)
                .ToArray();

            var chnl = e.Channel as SocketGuildChannel;
            if (chnl != null)
            {
                var guild = chnl.Guild;
                foreach (var userName in results)
                {             
                    if (e.Author.Username == userName)
                    {
                        var mention = $"<@{e.Author.Id}>";
                        response = response.Replace("@" + userName, mention);
                    }
                }
            }

            return response;
        }

        public bool alwaysRespond(SocketUserMessage e)
        {
            if (e.Channel.GetChannelType() == ChannelType.DM || e.MentionedUsers.Any(u => u.Username == discord.CurrentUser.Username) || e.Content.Contains(discord.CurrentUser.Username) || configuration.NickNames.Any(nickName => e.Content.ToLower().Contains(nickName.ToLower())))
            {
                return true;
            }
            return false;
        }

        public int getTypeTime(string message)
        {
            return message.Length * 80;
        }

        public async void defaultResponse(SocketUserMessage e)
        {
            await e.Channel.TriggerTypingAsync();
            var response = formatResponse(e, configuration.DefaultResponse);
            var typeTime = getTypeTime(response);
            await Task.Delay(typeTime).ContinueWith((task) => { e.ReplyAsync(response); });
        }

        public bool ignoreMessage(SocketUserMessage e)
        {
            if (e.MentionedUsers.Any(u => u.Username == discord.CurrentUser.Username))
            {
                return false;
            }
            if (e.Author.IsBot || e.Type != MessageType.Default || configuration.IgnoredChannels.Any(channel => channel == e.Channel.Name))
            {
                return true;
            }
            return false;
        }
    }
}
