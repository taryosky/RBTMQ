using System;
namespace RabbitMQConsumer.Dtos
{
    public class MongoDBConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string RequestLogs { get; set; }
    }
}

