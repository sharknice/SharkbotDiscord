using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.ImageGeneration
{
    public class ImageGenerationService
    {
        DiscordSocketClient discord;
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ImageGenerationService(DiscordSocketClient discordClient, HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botCnfiguration)
        {
            discord = discordClient;
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botCnfiguration;
        }

        public async Task<string> GenerateImageResponseAsync(SocketUserMessage e, string text, string userName)
        {
            var generationText = text.ToLower();

            if (string.IsNullOrWhiteSpace(userName))
            {
                if (text.ToLower().Contains("yourself"))
                {
                    userName = discord.CurrentUser.Username;
                    generationText.Replace("yourself", discord.CurrentUser.Username);
                }
                else
                {
                    var users = e.Channel.GetUsersAsync();
                    await foreach (var userCollection in users)
                    {
                        foreach (var user in userCollection)
                        {
                            if (text.ToLower().Contains(user.Username.ToLower()))
                            {
                                userName = user.Username;
                                break;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(userName)) { break; }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var userResponse = await client.GetAsync(configuration.ApiUrl + "/api/user/" + userName);
                userResponse.EnsureSuccessStatusCode();
                var userJsonResponse = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<UserData>(userJsonResponse);

                if (userData != null)
                {
                    var propertyText = GetProperties(userData);
                    if (!string.IsNullOrWhiteSpace(propertyText))
                    {
                        generationText += ", " + propertyText;
                    }
                }
            }

            dynamic request = new System.Dynamic.ExpandoObject();
            request.fn_index = 50;
            request.data = new dynamic[] 
                            {
                                generationText,
                                "",
                                "None",
                                "None",
                                20,
                                "Euler a",
                                true,
                                false,
                                1,
                                1,
                                7,
                                -1,
                                -1,
                                0,
                                0,
                                0,
                                false,
                                512,
                                512,
                                false,
                                0.7,
                                0,
                                0,
                                "None",
                                false,
                                false,
                                false,
                                "",
                                "Seed",
                                "",
                                "Nothing",
                                "",
                                true,
                                false,
                                false,
                                null,
                                ""
                            };

            var httpContent = utilityService.GetHttpContent(request);
            var response = await client.PostAsync(configuration.ImageApiUrl + "/run/predict/", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            ImageApiResponse result = JsonConvert.DeserializeObject<ImageApiResponse>(jsonResponse);

            var data = JsonConvert.DeserializeObject<ImageApiResponseData>(Convert.ToString(result.data[0]).Replace("\r\n", "").Replace("[", "").Replace("]", ""));
           
            return data.name;
        }

        string GetProperties(UserData userData)
        {
            var values = new List<string>();
            var properties = userData.properties.Concat(userData.derivedProperties).DistinctBy(p => p.name + p.value);
            var sex = properties.FirstOrDefault(p => p.name == "sex");
            if (sex != null)
            {
                values.Add(sex.value);
            }
            var derivedProperties = properties.Where(p => p.name != "sex");
            values.AddRange(derivedProperties.Where(p => p.name == "self").Select(p => p.value));
            values.AddRange(derivedProperties.Where(p => p.name != "self").Select(p => p.value + " " + p.name));

            return string.Join(", ", values);
        }
    }
}
