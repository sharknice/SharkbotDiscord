using System.Text.Json.Serialization;

namespace SharkbotDiscord.Models.Ollama
{
    public class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<OllamaMessage> Messages { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
    }
}
