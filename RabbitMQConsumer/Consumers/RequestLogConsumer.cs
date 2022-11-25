using System.Threading.Tasks;
using MassTransit;
using RabbitMQConsumer.Dtos;
using RabbitMQConsumer.Services.Abstractions;

namespace RabbitMQConsumer.Consumers
{
    public class RequestLogConsumer : IConsumer<HttpRequestDto>
    {
        private readonly IMongoDbService _mongoDbService;

        public RequestLogConsumer(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task Consume(ConsumeContext<HttpRequestDto> context)
        {
            await _mongoDbService.CreateAsync(context.Message);
        }
    }
}
