using System;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using RabbitMQConsumer.Dtos;

namespace RabbitMQConsumer.Services.Abstractions
{
    public interface IMongoDbService
    {
        Task CreateAsync(HttpRequestDto request);
    }
}

