using Discord;
using Discord.WebSocket;
using SharkbotDiscord.Services.Models;
using System.Text.RegularExpressions;

namespace SharkbotDiscord.Services.Bot
{
    public class SharkbotCommandService
    {
        BotConfiguration configuration;
        ChannelConfigurationService channelConfigurationService;

        public SharkbotCommandService(BotConfiguration botConfiguration, ChannelConfigurationService channelConfiguration)
        {
            configuration = botConfiguration;
            channelConfigurationService = channelConfiguration;
        }

        public bool command(SocketUserMessage e, Channel channel)
        {
            var message = e.Content.ToLower();

            var chnl = e?.Channel as SocketGuildChannel;
            if (chnl != null)
            {
                if (e.Channel != null && e.Author.Id == chnl.Guild.OwnerId)
                {
                    if (message.Contains("!quiet"))
                    {
                        channel.ChannelSettings.TargetedResponseConfidenceThreshold = 2;
                        var response = new Emoji("🤐");
                        e.AddReactionAsync(response);
                        channelConfigurationService.SaveChannel(channel);
                        return true;
                    }
                    else if (message.Contains("!talk"))
                    {
                        channel.ChannelSettings.TargetedResponseConfidenceThreshold = configuration.TargetedResponseConfidenceThreshold;
                        channelConfigurationService.SaveChannel(channel);
                        var response = new Emoji("👅");
                        e.AddReactionAsync(response);
                        e.ReplyAsync("hey 😉");
                        return true;
                    }
                    else if (SetTargetedResponseConfidenceThreshold(message, channel) || SetReactionConfidenceThreshold(message, channel))
                    {
                        var response = new Emoji("👍");
                        e.AddReactionAsync(response);
                    }
                }
            }

            return false;
        }

        private bool SetTargetedResponseConfidenceThreshold(string message, Channel channel)
        {
            Match match = Regex.Match(message, @"!threshold (^?[0-9]*\.?[0-9]+)$");

            if (match.Success)
            {
                var value = double.Parse(match.Groups[1].Value);
                channel.ChannelSettings.TargetedResponseConfidenceThreshold = value;
                channelConfigurationService.SaveChannel(channel);
                return true;
            }

            return false;
        }

        private bool SetReactionConfidenceThreshold(string message, Channel channel)
        {
            Match match = Regex.Match(message, @"!reaction (^?[0-9]*\.?[0-9]+)$");

            if (match.Success)
            {
                var value = double.Parse(match.Groups[1].Value);
                channel.ChannelSettings.ReactionConfidenceThreshold = value;
                channelConfigurationService.SaveChannel(channel);
                return true;
            }

            return false;
        }
    }
}
