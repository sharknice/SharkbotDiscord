using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.Ollama
{
    public class DirectedReplyCheckService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public DirectedReplyCheckService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botConfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botConfiguration;
        }

        /// <summary>
        /// between 0 and 2
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<double> DirectedReplyAsync(SocketUserMessage e)
        {
            var conversationName = utilityService.GetConversationName(e);
            var userName = e.Author.Username;
            var metadata = utilityService.GetMetadata(e);
            var response = await client.GetAsync(configuration.ApiUrl + "/api/conversation/discord/" + conversationName);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var conversationData = JsonConvert.DeserializeObject<ConversationData>(jsonResponse);
            var lastMessage = conversationData.responses.LastOrDefault();
            if (conversationData.responses.Count() > 1 && lastMessage != null && lastMessage.chat.user != lastMessage.chat.botName)
            {
                var secondToLastMessage = conversationData.responses[^2];
                if (secondToLastMessage != null && secondToLastMessage.botName == secondToLastMessage.chat.user)
                {
                    var confidence = secondToLastMessage.naturalLanguageData.responseConfidence;
                    if (lastMessage.naturalLanguageData.sentences.Any(s => s.SentenceType == SentenceType.Interrogative))
                    {
                        return confidence * 2;
                    }
                    return confidence;
                }
            }

            if (conversationData.responses.Count() == 1 && lastMessage.chat.user != lastMessage.chat.botName)
            {
                if (conversationData.groupChat)
                {
                    return 2;
                }
                return 1;
            }

            return 0;
        }
    }
}
