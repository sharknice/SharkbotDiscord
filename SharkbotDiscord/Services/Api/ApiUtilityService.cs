using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Models;
using System.Text;

namespace SharkbotDiscord.Services.Api
{
    public class ApiUtilityService
    {
        long launchTime;
        BotConfiguration configuration;

        public ApiUtilityService(BotConfiguration botConfiguration)
        {
            configuration = botConfiguration;
            launchTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public StringContent GetHttpContent(dynamic request)
        {
            var jsonString = JsonConvert.SerializeObject(request);
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        public Metadata GetMetadata(SocketUserMessage message)
        {
            var chnl = message.Channel as SocketGuildChannel;
            if (chnl != null)
            {
                return new Metadata { channelId = message.Channel.Id, guildId = chnl.Guild.Id };
            }
            return new Metadata { channelId = message.Channel.Id };
        }

        public Metadata GetMetadata(SocketReaction message)
        {
            var chnl = message.Channel as SocketGuildChannel;
            if (chnl != null)
            {
                return new Metadata { channelId = message.Channel.Id, guildId = chnl.Guild.Id };
            }
            return new Metadata { channelId = message.Channel.Id };
        }

        public Chat GetChat(SocketUserMessage discordMessage)
        {
            var message = discordMessage.Content;
            foreach (var mention in discordMessage.MentionedUsers)
            {
                message = message.Replace(mention.Mention.Replace("!", ""), "@" + mention.Username);
                message = message.Replace(mention.Mention, "@" + mention.Username);
            }
            return new Chat { botName = configuration.BotName, message = message, user = discordMessage.Author.Username, time = DateTimeOffset.Now.ToUnixTimeMilliseconds() };
        }

        public Chat GetChat(SocketReaction discordMessage)
        {
            var message = discordMessage.Emote.Name;
            return new Chat { botName = configuration.BotName, message = message, user = discordMessage.User.Value.Username, time = DateTimeOffset.Now.ToUnixTimeMilliseconds() };
        }

        public string GetConversationName(SocketUserMessage message)
        {
            string guildId = message.Channel.Name;
            var chnl = message.Channel as SocketGuildChannel;
            if (chnl != null)
            {
                guildId = chnl.Guild.Id.ToString();
            }

            return configuration.ChatType + "-discord-" + guildId + "-" + message.Channel.Id + "-" + launchTime;
        }

        public string GetConversationName(SocketReaction message)
        {
            string guildId = message.Channel.Name;
            var chnl = message.Channel as SocketGuildChannel;
            if (chnl != null)
            {
                guildId = chnl.Guild.Id.ToString();
            }

            return configuration.ChatType + "-discord-" + guildId + "-" + message.Channel.Id + "-" + launchTime;
        }
    }
}
