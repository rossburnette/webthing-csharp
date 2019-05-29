using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Mozilla.IoT.WebThing.AspNetCore.Extensions.Middlewares
{
    public class GetThingMiddleware : AbstractThingMiddleware
    {
        public GetThingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IThingType thingType) 
            : base(next, loggerFactory.CreateLogger<GetThingMiddleware>(), thingType)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket =  await httpContext.WebSockets.AcceptWebSocketAsync();
                var thingWebSocket = new ThingWebSocket(webSocket, thing);
                thing.AddSubscriber(thingWebSocket);
            }

            if (thing == null)
            {
                await NotFoundAsync(httpContext);
                return;
            }

            string ws = string.Empty;
            var link = new JObject(
                new JProperty("rel", "alternate"),
                new JProperty("href", ws));

            JObject description = thing.AsThingDescription();
            
            (description["links"] as JArray)?.Add(link);
            
            await OkAsync(httpContext, description);
        }
    }
}
