using Discord.WebSocket;
using MongoDB.Driver;
using SharkbotDiscord.Services.Models;
using Channel = SharkbotDiscord.Services.Models.Channel;

namespace SharkbotDiscord
{
    public class ChannelConfigurationService
    {
        BotConfiguration configuration;
        IMongoDatabase database;
        IMongoCollection<Channel> channelsCollection;

        public ChannelConfigurationService(BotConfiguration botConfiguration)
        {
            configuration = botConfiguration;

            if (!string.IsNullOrEmpty(configuration.MongoDbConnectionString))
            {
                var mongoClient = new MongoClient(configuration.MongoDbConnectionString);
                database = mongoClient.GetDatabase("discord");
                channelsCollection = database.GetCollection<Channel>("channels");
            }
        }

        public Channel GetChannelConfiguration(SocketUserMessage e)
        {
            if (!configuration.Channels.Any(c => c.ChannelId == e.Channel.Id))
            {
                if(e.Channel.Name == null)
                {
                    return new Channel { ChannelSettings = new ChannelSettings { ReactionConfidenceThreshold = 0, TargetedResponseConfidenceThreshold = 0 } };
                }

                var channel = new Channel
                {
                    ChannelId = e.Channel.Id,
                    Name = e.Channel.Id + "#" + e.Channel.Name,
                    ChannelSettings = new ChannelSettings
                    {
                        ReactionConfidenceThreshold = configuration.ReactionConfidenceThreshold,
                        TargetedResponseConfidenceThreshold = configuration.TargetedResponseConfidenceThreshold
                    }
                };
                configuration.Channels.Add(channel);
                if(channelsCollection != null)
                {
                    channelsCollection.InsertOne(channel);
                }              
            }
            return configuration.Channels.Find(c => c.ChannelId == e.Channel.Id);
        }

        public List<Channel> LoadChannels()
        {
            if (channelsCollection == null)
            {
                return new List<Channel>();
            }
            return channelsCollection.Find(c => true).ToList();
        }

        public async void SaveChannel(Channel channel)
        {
            if (channelsCollection == null)
            {
                return;
            }
            var filter = Builders<Channel>.Filter.Eq(c => c._id, channel._id);
            var update = Builders<Channel>.Update.Set(c => c.ChannelSettings, channel.ChannelSettings);
            await channelsCollection.UpdateOneAsync(filter, update);
        }
    }
}
