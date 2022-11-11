using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.Api
{
    public class ChatUpdateService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ChatUpdateService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botCnfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botCnfiguration;
        }

        public async Task<bool> UpdateChatAsync(SocketUserMessage e)
        {
            var chat = utilityService.GetChat(e);
            var conversationName = utilityService.GetConversationName(e);
            var metadata = utilityService.GetMetadata(e);
            var chatRequest = new ChatRequest { chat = chat, type = configuration.ChatType, conversationName = conversationName, metadata = metadata };

            var httpContent = utilityService.GetHttpContent(chatRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/chatupdate", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var success = JsonConvert.DeserializeObject<bool>(jsonResponse);

            return success;
        }
    }
}
