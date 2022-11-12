using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace SharkbotDiscord.Services.ImageGeneration
{
    public class ImageResponseUtility
    {
        private List<string> drawMe = new List<string>() { "draw me with (.*)", "draw me on (.*)", "draw me in (.*)", "draw me at (.*)", "draw me as (.*)" };
        private List<string> drawForMe = new List<string>() { "draw me a (.*)", "draw me an (.*)", "draw me the (.*)" };
        private List<string> drawSomething = new List<string>() { "draw me (.*)", "draw (.*)", "show me (.*)" };

        public ImageResponseUtility()
        {

        }

        public GenerationData AskingForImageResponse(SocketUserMessage e)
        {
            var text = e.Content.ToLower();
            foreach (var regex in drawMe)
            {
                var match = getMatch(text, regex);
                if (!string.IsNullOrWhiteSpace(match))
                {
                    return new GenerationData(e.Author.Username, text.Replace("me", e.Author.Username));
                }
            }
            foreach (var regex in drawForMe)
            {
                var match = getMatch(text, regex);
                if (!string.IsNullOrWhiteSpace(match))
                {
                    return new GenerationData(null, match);
                }
            }

            if (e.Content.ToLower().EndsWith("draw me"))
            {
                return new GenerationData(e.Author.Username, e.Author.Username);
            }

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
