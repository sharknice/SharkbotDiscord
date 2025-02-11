﻿using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using SharkbotDiscord.Configuration;
using SharkbotDiscord.Services.Api;
using SharkbotDiscord.Services.Bot;
using SharkbotDiscord.Services.ImageGeneration;
using SharkbotDiscord.Services.Models;
using SharkbotDiscord.Services.MusicGeneration;
using SharkbotDiscord.Services.Ollama;

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
        static OllamaResponseService ollamaResponseService;
        static DirectedReplyCheckService directedReplyCheckService;
        static ChatUpdateService chatUpdateService;
        static ChannelConfigurationService channelbotConfiguration;
        static BotUtilityService botUtilityService;
        static SharkbotCommandService sharkbotCommandService;
        static BotReactionService botReactionService;
        static RequiredPropertyResponseService requiredPropertyResponseService;
        static ImageResponseUtility imageResponseUtility;
        static GenerateImageResponseService generateImageResponseService;
        static EmojiService emojiService;
        static ImageGenerationService imageGenerationService;
        static MusicResponseUtility musicResponseUtility;
        static MusicGenerationService musicGenerationService;
        static GenerateMusicResponseService generateMusicResponseService;
        static RequiredSettingsLoader requiredSettingsLoader;
        static OptionalSettingsLoader optionalSettingsLoader;

        static BotConfiguration botConfiguration;
        static Random random;

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
            botConfiguration = new BotConfiguration();
            requiredSettingsLoader = new RequiredSettingsLoader();
            optionalSettingsLoader = new OptionalSettingsLoader();
            botConfiguration = requiredSettingsLoader.LoadRequiredSettings(config, botConfiguration);
            botConfiguration = optionalSettingsLoader.LoadOptionalSettings(config, botConfiguration);        
            botConfiguration.Channels = new List<Channel>();//TODO: load channel settings from mongodb
            channelbotConfiguration = new ChannelConfigurationService(botConfiguration);
            botConfiguration.Channels = channelbotConfiguration.LoadChannels();
            var apiUtilityService = new ApiUtilityService(botConfiguration);

            chatResponseService = new ChatResponseService(client, apiUtilityService, botConfiguration);
            ollamaResponseService = new OllamaResponseService(client, apiUtilityService, botConfiguration);
            directedReplyCheckService = new DirectedReplyCheckService(client, apiUtilityService, botConfiguration);
            chatUpdateService = new ChatUpdateService(client, apiUtilityService, botConfiguration);
            userDetailService = new UserDetailService(client, botConfiguration);
            reactionService = new ReactionService(client, apiUtilityService, botConfiguration);
            reactionAddService = new ReactionAddService(client, apiUtilityService, botConfiguration);
            imageResponseUtility = new ImageResponseUtility();
            imageGenerationService = new ImageGenerationService(discord, client, apiUtilityService, botConfiguration);
            generateImageResponseService = new GenerateImageResponseService();
            emojiService = new EmojiService(discord);

            musicResponseUtility = new MusicResponseUtility();
            musicGenerationService = new MusicGenerationService(discord, client, apiUtilityService, botConfiguration);
            generateMusicResponseService = new GenerateMusicResponseService();

            botUtilityService = new BotUtilityService(_discord, botConfiguration);
            sharkbotCommandService = new SharkbotCommandService(botConfiguration, channelbotConfiguration);
            botReactionService = new BotReactionService(botConfiguration, new EmojiService(_discord));
            requiredPropertyResponseService = new RequiredPropertyResponseService(_discord, botUtilityService, emojiService);

            random = new Random();

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
            else if (sharkbotCommandService.command(msg, channel) || imageResponseUtility.AskingForImageResponse(msg) != null || musicResponseUtility.AskingForMusicResponse(msg) != null)
            {
                var imageGenerationText = imageResponseUtility.AskingForImageResponse(msg);
                if (imageGenerationText != null)
                {
                    var imagePath = await imageGenerationService.GenerateImageResponseAsync(msg, imageGenerationText.Text, imageGenerationText.UserName);
                    generateImageResponseService.GenerateImageResponse(msg, imagePath);
                }
                var musicGenerationText = musicResponseUtility.AskingForMusicResponse(msg);
                if (musicGenerationText != null)
                {
                    var musicPath = await musicGenerationService.GenerateMusicResponseAsync(msg, musicGenerationText.Text, musicGenerationText.UserName);
                    generateMusicResponseService.GenerateMusicResponse(msg, musicPath);
                }
            }
            else if (!botUtilityService.ignoreMessage(msg))
            {
                var reaction = await reactionService.GetReactionAsync(msg);
                botReactionService.reactionResponse(msg, reaction, channel.ChannelSettings);

                if (!string.IsNullOrEmpty(botConfiguration.OllamaApiUrl))
                {
                    await chatUpdateService.UpdateChatAsync(msg);

                    var hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(msg);
                    var randomChance = random.NextDouble();
                    if (hasRequiredProperty || botConfiguration.OllamaChance > randomChance)
                    {
                        var ollamaChatResponse = await ollamaResponseService.GetChatResponseAsync(msg);
                        requiredPropertyResponseService.hasRequiredPropertyResponse(msg, ollamaChatResponse, channel.ChannelSettings);
                    }
                    else
                    {
                        var directedReplyConfidence = await directedReplyCheckService.DirectedReplyAsync(msg);
                        if (botConfiguration.OllamaReplyChance * directedReplyConfidence > randomChance)
                        {
                            var ollamaChatResponse = await ollamaResponseService.GetChatResponseAsync(msg);
                            requiredPropertyResponseService.hasRequiredPropertyResponse(msg, ollamaChatResponse, channel.ChannelSettings);
                        }
                    }
                }
                else
                {
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
            }

            string tag = $"#{msg.Channel.Name}";
            var chnl = msg.Channel as SocketGuildChannel;
            if (chnl != null)
            {
                tag = $"{chnl.Guild.Name}{tag}";
            }

            Console.WriteLine($"{tag} {msg.Author.Username}: {msg}");
        }

        async void UpdateStatus()
        {
            var memberCount = _discord.Guilds.Sum(guild => guild.MemberCount);
            await _discord.SetGameAsync($"with {memberCount} ♥s");
        }
    }
}
