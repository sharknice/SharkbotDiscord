using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Models.Ollama;
using SharkbotDiscord.Services.Api;
using SharkbotDiscord.Services.Models;
using System.Text.RegularExpressions;

namespace SharkbotDiscord.Services.Ollama
{
    public class OllamaResponseService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public OllamaResponseService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botConfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botConfiguration;
        }

        public async Task<ChatResponse> GetChatResponseAsync(SocketUserMessage e)
        {
            var conversationName = utilityService.GetConversationName(e);
            var userName = e.Author.Username;
            var metadata = utilityService.GetMetadata(e);
            var response = await client.GetAsync(configuration.ApiUrl + "/api/conversation/discord/" + conversationName);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var conversationData = JsonConvert.DeserializeObject<ConversationData>(jsonResponse);

            // https://github.com/ollama/ollama/blob/main/docs/api.md#generate-a-chat-completion
            var messages = new List<OllamaMessage>
            {
                new OllamaMessage { Role = "system", Content = configuration.OllamaSystemPrompt },
                new OllamaMessage { Role = "system", Content = $"Your name is {configuration.BotName}" }
            };

            if (conversationData.groupChat)
            {
                messages.Add(new OllamaMessage { Role = "system", Content = $"You are participating in a group chat." });
            }
            else
            {
                messages.Add(new OllamaMessage { Role = "system", Content = $"You are talking with user:{userName}." });
            }

            var userResponse = await client.GetAsync(configuration.ApiUrl + "/api/user/" + userName);
            userResponse.EnsureSuccessStatusCode();
            var userJsonResponse = await userResponse.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(userJsonResponse);
            if (userData.derivedProperties != null)
            {
                foreach (var userProperty in userData.derivedProperties.DistinctBy(p => p.name + p.value))
                {
                    messages.Add(new OllamaMessage { Role = "system", Content = $"{userName}'s {userProperty.name} is {userProperty.value}" });
                }
            }

            messages.Add(new OllamaMessage { Role = "system", Content = $"The time is {DateTime.Now.ToString("F")}" });

            foreach (var chat in conversationData.responses)
            {
                if (chat.chat.user == chat.botName)
                {
                    var message = new OllamaMessage { Role = "assistant", Content = chat.chat.message };
                    messages.Add(message);
                }
                else
                {
                    var content = chat.chat.message;
                    if (conversationData.groupChat)
                    {
                        content = $"{chat.chat.user}: " + content;
                    }
                    var message = new OllamaMessage { Role = "user", Content = content };
                    messages.Add(message);
                }
            }

            var chatRequest = new OllamaChatRequest
            {
                Model = configuration.OllamaModel,
                Messages = messages,
                Stream = false
            };
            var httpContent = utilityService.GetHttpContent(chatRequest);
            var ollamaResponse = await client.PostAsync(configuration.OllamaApiUrl + "/api/chat", httpContent);
            ollamaResponse.EnsureSuccessStatusCode();
            var ollamaJsonResponse = await ollamaResponse.Content.ReadAsStringAsync();
            var ollamaChatResponse = JsonConvert.DeserializeObject<OllamaChatResponse>(ollamaJsonResponse);

            var responseMessage = RemoveThinkTags(ollamaChatResponse.Message.Content);

            var chatResponse = new ChatResponse
            {
                response = new List<string> { responseMessage },
                metadata = metadata,
                confidence = configuration.OllamaConfidence
            };

            return chatResponse;
        }

        string RemoveThinkTags(string input)
        {
            var message = Regex.Replace(input, @"<think>.*?</think>", string.Empty, RegexOptions.Singleline);
            return message.Trim();
        }
    }
}
