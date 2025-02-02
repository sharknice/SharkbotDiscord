using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.Api
{
    public class ReactionAddService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ReactionAddService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botConfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botConfiguration;
        }

        public async Task<bool> AddReactionAsync(SocketReaction e)
        {
            var reaction = new Reaction();
            reaction.reaction = e.Emote.Name;
            reaction.user = e.User.Value.Username;
            reaction.time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var chat = utilityService.GetChat(e);
            var conversationName = utilityService.GetConversationName(e);
            var metadata = utilityService.GetMetadata(e);
            var reactionRequest = new ReactionRequest { reaction = reaction, chat = chat, type = configuration.ChatType, conversationName = conversationName, metadata = metadata };

            var httpContent = utilityService.GetHttpContent(reactionRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/reactionupdate", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var success = JsonConvert.DeserializeObject<bool>(jsonResponse);

            return success;
        }
    }
}
