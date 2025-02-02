using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.Api
{
    public class UserDetailService
    {
        HttpClient client;
        BotConfiguration configuration;

        public UserDetailService(HttpClient httpClient, BotConfiguration botConfiguration)
        {
            client = httpClient;
            configuration = botConfiguration;
        }

        public async Task<bool> HasRequiredPropertyAsync(SocketUserMessage e)
        {
            var userName = e.Author.Username;
            var response = await client.GetAsync(configuration.ApiUrl + "/api/user/" + userName);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(jsonResponse);

            if (!configuration.RequiredProperyMatches.Any()) { return false; }
            return configuration.RequiredProperyMatches.All(rp => userData.derivedProperties.Any(dp => dp.name == rp));
        }
    }
}
