using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Services.Api;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Services.ImageGeneration
{
    public class ImageGenerationService
    {
        HttpClient client;
        ApiUtilityService utilityService;
        BotConfiguration configuration;

        public ImageGenerationService(HttpClient httpClient, ApiUtilityService apiUtilityService, BotConfiguration botCnfiguration)
        {
            client = httpClient;
            utilityService = apiUtilityService;
            configuration = botCnfiguration;
        }

        public async Task<string> GenerateImageResponseAsync(SocketUserMessage e)
        {
            var text = "ninja sword";
            dynamic request = new System.Dynamic.ExpandoObject();
            request.fn_index = 50;
            request.data = new dynamic[] 
                            {
                                text,
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

            //var request = $"fn_index: 50, data: [ {text}, '', 'None', 'None', 20, 'Euler a', true, false, 1, 1, 17, -1, -1, 0, 0 , 0, false, 512, 512, false, 0.7, 0, 0, 'none', false, false, false, '', 'Seed', '', 'Nothing', '', true, false, false, null, 'test' ]";
            var httpContent = utilityService.GetHttpContent(request);
            var response = await client.PostAsync(configuration.ImageApiUrl + "/run/predict/", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            ImageApiResponse result = JsonConvert.DeserializeObject<ImageApiResponse>(jsonResponse);

            var data = JsonConvert.DeserializeObject<ImageApiResponseData>(Convert.ToString(result.data[0]).Replace("\r\n", "").Replace("[", "").Replace("]", ""));
           

            return data.name;
        }
    }
}
