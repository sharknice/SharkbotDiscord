using Microsoft.Extensions.Configuration;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Configuration
{
    public class RequiredSettingsLoader
    {
        public BotConfiguration LoadRequiredSettings(IConfiguration configuration, BotConfiguration botConfiguration)
        {     
            botConfiguration.Token = configuration.GetSection("Token").Value;
            botConfiguration.ApiUrl = configuration.GetSection("ApiUrl").Value;
            botConfiguration.ImageApiUrl = configuration.GetSection("ImageApiUrl").Value;
            botConfiguration.ImageApiDirectory = configuration.GetSection("ImageApiDirectory").Value;
            botConfiguration.MusicApiUrl = configuration.GetSection("MusicApiUrl").Value;
            botConfiguration.BotName = configuration.GetSection("BotName").Value;
            botConfiguration.ChatType = configuration.GetSection("ChatType").Value;

            return botConfiguration;
        }
    }
}
