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

            return botConfiguration;
        }
    }
}
