using Microsoft.Extensions.Configuration;
using SharkbotDiscord.Services.Models;

namespace SharkbotDiscord.Configuration
{
    public class OptionalSettingsLoader
    {
        public BotConfiguration LoadOptionalSettings(IConfiguration configuration, BotConfiguration botConfiguration)
        {
            botConfiguration.IgnoredChannels = configuration.GetSection("IgnoredChannels").Get<List<string>>();
            if (botConfiguration.IgnoredChannels == null)
            {
                botConfiguration.IgnoredChannels = new List<string>();
            }

            botConfiguration.ExclusiveTypes = configuration.GetSection("ExclusiveTypes").Get<List<string>>();
            if (botConfiguration.ExclusiveTypes == null)
            {
                botConfiguration.ExclusiveTypes = new List<string>();
            }

            botConfiguration.RequiredProperyMatches = configuration.GetSection("RequiredProperyMatches").Get<List<string>>();
            if (botConfiguration.RequiredProperyMatches == null)
            {
                botConfiguration.RequiredProperyMatches = new List<string>();
            }

            botConfiguration.NickNames = configuration.GetSection("NickNames").Get<List<string>>();
            if (botConfiguration.NickNames == null)
            {
                botConfiguration.NickNames = new List<string>();
            }

            var targetedResponseConfidenceThreshold = configuration.GetSection("TargetedResponseConfidenceThreshold");
            botConfiguration.TargetedResponseConfidenceThreshold = 0;
            if (targetedResponseConfidenceThreshold.Value != null)
            {
                botConfiguration.TargetedResponseConfidenceThreshold = double.Parse(targetedResponseConfidenceThreshold.Value);
            }

            var reactionConfidenceThreshold = configuration.GetSection("ReactionConfidenceThreshold");
            botConfiguration.ReactionConfidenceThreshold = 0;
            if (reactionConfidenceThreshold.Value != null)
            {
                botConfiguration.ReactionConfidenceThreshold = double.Parse(reactionConfidenceThreshold.Value);
            }

            var maximumReactionsPerMessage = configuration.GetSection("MaximumReactionsPerMessage");
            botConfiguration.MaximumReactionsPerMessage = 1;
            if (maximumReactionsPerMessage.Value != null)
            {
                botConfiguration.MaximumReactionsPerMessage = int.Parse(maximumReactionsPerMessage.Value);
            }

            botConfiguration.DefaultResponse = configuration.GetSection("DefaultResponse").Value;
            var defaultResponse = configuration.GetSection("DefaultResponse");
            botConfiguration.DefaultResponse = string.Empty;
            if (defaultResponse.Value != null)
            {
                botConfiguration.DefaultResponse = defaultResponse.Value;
            }

            botConfiguration.MongoDbConnectionString = null;
            var connectionStringSection = configuration.GetSection("MongoDbConnectionString");
            if (connectionStringSection.Value != null)
            {
                botConfiguration.MongoDbConnectionString = connectionStringSection.Value;
            }

            var ollamaApiUrl = configuration.GetSection("OllamaApiUrl");
            botConfiguration.DefaultResponse = string.Empty;
            if (ollamaApiUrl.Value != null)
            {
                botConfiguration.OllamaApiUrl = ollamaApiUrl.Value;
            }

            var ollamaModel = configuration.GetSection("OllamaModel");
            botConfiguration.OllamaModel = string.Empty;
            if (ollamaModel.Value != null)
            {
                botConfiguration.OllamaModel = ollamaModel.Value;
            }

            var ollamaSystemPrompt = configuration.GetSection("OllamaSystemPrompt");
            botConfiguration.OllamaSystemPrompt = string.Empty;
            if (ollamaSystemPrompt.Value != null)
            {
                botConfiguration.OllamaSystemPrompt = ollamaSystemPrompt.Value;
            }

            var ollamaConfidence = configuration.GetSection("OllamaConfidence");
            botConfiguration.OllamaConfidence = 0;
            if (ollamaConfidence.Value != null)
            {
                botConfiguration.OllamaConfidence = double.Parse(ollamaConfidence.Value);
            }

            var ollamaChance = configuration.GetSection("OllamaChance");
            botConfiguration.OllamaChance = 0;
            if (ollamaChance.Value != null)
            {
                botConfiguration.OllamaChance = double.Parse(ollamaChance.Value);
            }

            var ollamaReplyChance = configuration.GetSection("OllamaReplyChance");
            botConfiguration.OllamaReplyChance = 0;
            if (ollamaReplyChance.Value != null)
            {
                botConfiguration.OllamaReplyChance = double.Parse(ollamaReplyChance.Value);
            }

            return botConfiguration;
        }
    }
}
