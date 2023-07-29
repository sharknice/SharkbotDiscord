using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using SharkbotDiscord.Models.Api;
using SharkbotDiscord.Services.Api;
using SharkbotDiscord.Services.Models;
using System.Text;

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
                if (generationText.ToLower().Contains("yourself"))
                {
                    userName = discord.CurrentUser.Username;
                    generationText = generationText.Replace("yourself", discord.CurrentUser.Username);
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

            var request = CreateRequest(generationText);
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(configuration.ImageApiUrl + "/sdapi/v1/txt2img", content);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ImageApiResponse>(jsonResponse);

            var fileName = SaveImage(result.images[0]);

            return fileName;
        }

        GenerationRequest CreateRequest(string generationText)
        {
            return new GenerationRequest
            {
                enable_hr = false,
                denoising_strength = 0,
                firstphase_width = 0,
                firstphase_height = 0,
                hr_scale = 2,
                hr_upscaler = "None",
                hr_second_pass_steps = 0,
                hr_resize_x = 0,
                hr_resize_y = 0,
                hr_sampler_name = "",
                hr_prompt = "",
                hr_negative_prompt = "",
                prompt = generationText,
                styles = new List<string>(),
                seed = -1,
                subseed = -1,
                subseed_strength = 0,
                seed_resize_from_h = -1,
                seed_resize_from_w = -1,
                sampler_name = "Euler a",
                batch_size = 1,
                n_iter = 1,
                steps = 50,
                cfg_scale = 7,
                width = 1024,
                height = 1024,
                restore_faces = false,
                tiling = false,
                do_not_save_samples = false,
                do_not_save_grid = false,
                negative_prompt = "",
                eta = 0,
                s_min_uncond = 0,
                s_churn = 0,
                s_tmax = 0,
                s_tmin = 0,
                s_noise = 1,
                override_settings = { },
                override_settings_restore_afterwards = true,
                script_args = new List<object>(),
                sampler_index = "Euler",
                script_name = "",
                send_images = true,
                save_images = false,
                alwayson_scripts = new AlwaysonScripts()
            };
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

        public string SaveImage(string base64imageString)
        {
            Directory.CreateDirectory("images");
            string filePath = "images/" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() + ".png";
            File.WriteAllBytes(filePath, Convert.FromBase64String(base64imageString));
            return filePath;
        }
    }
}
