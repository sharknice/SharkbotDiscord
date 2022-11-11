using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using SharkbotDiscord.Configuration;
using SharkbotDiscord.Services.Api;
using SharkbotDiscord.Services.Bot;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;

        static UserDetailService userDetailService;
        static ReactionService reactionService;
        static ReactionAddService reactionAddService;
        static ChatResponseService chatResponseService;
        static ChatUpdateService chatUpdateService;
        static ChannelConfigurationService channelbotConfiguration;
        static BotUtilityService botUtilityService;
        static SharkbotCommandService sharkbotCommandService;
        static BotReactionService botReactionService;
        static RequiredPropertyResponseService requiredPropertyResponseService;
        static RequiredSettingsLoader requiredSettingsLoader;
        static OptionalSettingsLoader optionalSettingsLoader;

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;

            var client = new HttpClient();
            var botConfiguration = new BotConfiguration();
            requiredSettingsLoader = new RequiredSettingsLoader();
            optionalSettingsLoader = new OptionalSettingsLoader();
            botConfiguration = requiredSettingsLoader.LoadRequiredSettings(config, botConfiguration);
            botConfiguration = optionalSettingsLoader.LoadOptionalSettings(config, botConfiguration);        
            botConfiguration.Channels = new List<Channel>();//TODO: load channel settings from mongodb
            channelbotConfiguration = new ChannelConfigurationService(botConfiguration);
            botConfiguration.Channels = channelbotConfiguration.LoadChannels();
            var apiUtilityService = new ApiUtilityService(botConfiguration);

            chatResponseService = new ChatResponseService(client, apiUtilityService, botConfiguration);
            chatUpdateService = new ChatUpdateService(client, apiUtilityService, botConfiguration);
            userDetailService = new UserDetailService(client, botConfiguration);
            reactionService = new ReactionService(client, apiUtilityService, botConfiguration);
            reactionAddService = new ReactionAddService(client, apiUtilityService, botConfiguration);

            botUtilityService = new BotUtilityService(_discord, botConfiguration);
            sharkbotCommandService = new SharkbotCommandService(botConfiguration, channelbotConfiguration);
            botReactionService = new BotReactionService(botConfiguration, new EmojiService(_discord));
            requiredPropertyResponseService = new RequiredPropertyResponseService(_discord, botUtilityService);

            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
            if (msg == null) return;

            var channel = channelbotConfiguration.GetChannelConfiguration(msg);
            UpdateStatus();

            if (msg.Author.Id == _discord.CurrentUser.Id)
            {
                await chatUpdateService.UpdateChatAsync(msg);
            }
            else if (sharkbotCommandService.command(msg, channel))
            {

            }
            else if (!botUtilityService.ignoreMessage(msg))
            {
                var reaction = await reactionService.GetReactionAsync(msg);
                botReactionService.reactionResponse(msg, reaction, channel.ChannelSettings);

                var hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(msg);
                if (hasRequiredProperty)
                {
                    var chatResponse = await chatResponseService.GetChatResponseAsync(msg);
                    requiredPropertyResponseService.hasRequiredPropertyResponse(msg, chatResponse, channel.ChannelSettings);
                }
                else
                {
                    var chatResponse = await chatResponseService.GetChatResponseAsync(msg);
                    hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(msg);
                    if (!hasRequiredProperty && botUtilityService.alwaysRespond(msg))
                    {
                        botUtilityService.defaultResponse(msg);
                    }
                    else
                    {
                        requiredPropertyResponseService.hasRequiredPropertyResponse(msg, chatResponse, channel.ChannelSettings);
                    }
                }
            }

            var context = new SocketCommandContext(_discord, msg);     // Create the command context

            int argPos = 0;     // Check if the message has a valid command prefix

            Console.WriteLine(msg);

            if (msg.HasStringPrefix(_config["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)     // If not successful, reply with the error.
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }

        async void UpdateStatus()
        {
            var memberCount = _discord.Guilds.Sum(guild => guild.MemberCount);
            await _discord.SetGameAsync($"with {memberCount} ♥s");
        }
    }
}
