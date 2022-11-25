using Microsoft.Extensions.Options;
using RabbitMQConsumer.Dtos;
using System.Threading.Tasks;
using RabbitMQConsumer.Services.Abstractions;
using MongoDB.Driver;

namespace RabbitMQConsumer.Services.Implementations
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<HttpRequestDto> _requests;
        public MongoDbService(IOptions<MongoDBConfig> mongoConfig)
        {
            var mongodbClient = new MongoClient(mongoConfig.Value.ConnectionString);
            var database = mongodbClient.GetDatabase(mongoConfig.Value.DatabaseName);

            _requests = database.GetCollection<HttpRequestDto>(mongoConfig.Value.RequestLogs);
        }

        public async Task CreateAsync(HttpRequestDto request)
            => await _requests.InsertOneAsync(request);
    }
}

