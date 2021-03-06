using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing
{
    internal class HttpPipeWriter : IHttpBodyWriter
    {
        private readonly IJsonSerializer _serializer;
        private readonly IJsonSerializerSettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public HttpPipeWriter(IJsonSerializer serializer, IJsonSerializerSettings settings, IHttpContextAccessor httpContextAccessor)
        {
            _serializer = serializer;
            _settings = settings;
            _httpContextAccessor = httpContextAccessor;
        }

        public ValueTask WriteAsync<T>(T value, HttpStatusCode httpStatusCode, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask WriteAsync<T>(T value, CancellationToken cancellationToken = default)
        {
            var buffer = _serializer.Serialize(value);
            
            var response = _httpContextAccessor.HttpContext.Response;
            response.ContentType = "application/json";

            var pipe = response.BodyWriter;
            var flushResult = await pipe.WriteAsync(buffer, cancellationToken);
            if (!flushResult.IsCompleted)
            {
                await pipe.FlushAsync(cancellationToken);
            }
        }
    }
}
