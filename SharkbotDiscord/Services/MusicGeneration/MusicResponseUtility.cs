using Discord.WebSocket;
using SharkbotDiscord.Services.ImageGeneration;
using System.Text.RegularExpressions;

namespace SharkbotDiscord.Services.MusicGeneration
{
    public class MusicResponseUtility
    {
        private List<string> drawSomething = new List<string>() { "compose me (.*)", "compose (.*)", "make a song (.*)", "make music (.*)" };

        public MusicResponseUtility()
        {

        }

        public GenerationData AskingForMusicResponse(SocketUserMessage e)
        {
            var text = e.Content.ToLower();

            foreach (var regex in drawSomething)
            {
                var match = getMatch(text, regex);
                if (!string.IsNullOrWhiteSpace(match))
                {
                    return new GenerationData(null, match);
                }
            }

            return null;
        }

        string getMatch(string message, string regex)
        {
            var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);
            if (match.Groups.Count == 2)
            {
                return match.Groups[1].Value;
            }
            return null;
        }
    }
}
