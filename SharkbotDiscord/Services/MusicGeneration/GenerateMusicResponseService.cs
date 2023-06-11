using Discord;
using Discord.WebSocket;

namespace SharkbotDiscord.Services.ImageGeneration
{
    public class GenerateMusicResponseService
    {
        public GenerateMusicResponseService()
        {
        }

        public async void GenerateMusicResponse(SocketUserMessage e, string musicPath)
        {
            var filename = musicPath.Replace("audio.wav", "").Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() + ".wav";
            Directory.CreateDirectory("music");
            File.Copy(musicPath, "music/" + filename);

            await e.Channel.SendFileAsync("music/" + filename, null, false, messageReference: new MessageReference(e.Id));
        }
    }
}
