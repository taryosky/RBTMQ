using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using RabbitMQConsumer.Dtos;

namespace RabbitMQPublisher.Services
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IServiceProvider services;
        private readonly RecyclableMemoryStreamManager _recyclablememorystreammanager;

        public LoggerMiddleware(RequestDelegate next, IServiceProvider services)
        {
            this.next = next;
            this.services = services;
            _recyclablememorystreammanager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var httpRequest = new HttpRequestDto
            {
                Id = System.Guid.NewGuid().ToString(),
                Controller = (string)context.Request.RouteValues["controller"],
                Action = (string)context.Request.RouteValues["action"],
                Endpoint = context.Request.Path,
                RequestDate = DateTime.Now,
                HttpMethod = context.Request.Method,
            };

            if (context.Request.Query.Count() > 0)
            {
                var queryStrings = string.Join(",", context.Request.Query.Select(x => "{Key = " + x.Key + ", Value = " + x.Value + "}"));
                httpRequest.QueryStrings = queryStrings;
            }
            var body = new MemoryStream();

            if (context.Request.Method != "GET")
            {
                httpRequest.RequestPayload = await GetRequestBody(context.Request);
            }

            var originalResponse = context.Response.Body;
            using var resBody = _recyclablememorystreammanager.GetStream();
            context.Response.Body = resBody;

            await next(context);

            httpRequest.ResponseTime = (DateTime.Now - httpRequest.RequestDate).TotalSeconds;

            httpRequest.StatusCode = context.Response.StatusCode;

            httpRequest.Response = await GetResponse(context.Response, originalResponse);

            var publishEndpoint = services.CreateScope().ServiceProvider.GetRequiredService<IPublishEndpoint>();
            publishEndpoint.Publish(httpRequest);
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            var streamReader = new StreamReader(request.Body);
            var body = await streamReader.ReadToEndAsync();

            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            request.Body = memStream;
            return body;
        }

        private async Task<string> GetResponse(HttpResponse newResponse, Stream originalResponse)
        {
            newResponse.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(newResponse.Body).ReadToEndAsync();
            newResponse.Body.Seek(0, SeekOrigin.Begin);
            await newResponse.Body.CopyToAsync(originalResponse);
            return text;
        }
    }
}

