using System;
namespace RabbitMQConsumer.Dtos
{
    public class HttpRequestDto
    {
        public string Id { get; set; }
        public string HttpMethod { get; set; }
        public string Endpoint { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string RequestPayload { get; set; }
        public string QueryStrings { get; set; }
        public string Response { get; set; }
        public int StatusCode { get; set; }
        public DateTime RequestDate { get; set; }
        public double ResponseTime { get; set; }
    }
}

