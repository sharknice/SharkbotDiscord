using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.Api
{
    public class ChatResponseService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ChatResponseService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botCnfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botCnfiguration;
        }

        public async Task<ChatResponse> GetChatResponseAsync(SocketUserMessage e)
        {
            var chat = utilityService.GetChat(e);
            var conversationName = utilityService.GetConversationName(e);
            var metadata = utilityService.GetMetadata(e);
            var chatRequest = new ChatRequest { chat = chat, type = configuration.ChatType, conversationName = conversationName, metadata = metadata, requestTime = DateTime.Now, exclusiveTypes = configuration.ExclusiveTypes, requiredPropertyMatches = configuration.RequiredProperyMatches };

            var httpContent = utilityService.GetHttpContent(chatRequest);
            var response = await client.PutAsync(configuration.ApiUrl + "/api/chat", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);

            return chatResponse;
        }
    }
}
