using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Activator;

namespace Mozilla.IoT.WebThing.Endpoints
{
    internal sealed class PutProperty
    {
        internal static async Task Invoke(HttpContext httpContext)
        {
            var services = httpContext.RequestServices;
            var logger = services.GetRequiredService<ILogger<PutProperty>>();
            
            var route = services.GetRequiredService<IHttpRouteValue>();
            var thingId = route.GetValue<string>("thing");
            var propertyName = route.GetValue<string>("name");
            logger.LogInformation($"Put Property is calling: [[thing: {thingId}][property: {propertyName}]]");
            
            var thing = services.GetService<IThingActivator>()
                .CreateInstance(services, thingId);
            
            if (thing == null)
            {
                logger.LogInformation($"Put Property: Thing not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            
            var reader = services.GetRequiredService<IHttpBodyReader>();
            var json = await reader.ReadAsync<IDictionary<string, object>>();
            if (json == null)
            {
                logger.LogInformation($"Put Property: Body not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }

            if (!json.Keys.Any()) 
            {
                logger.LogInformation($"Put Property: Body is empty [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }

            var property = thing.Properties.FirstOrDefault(x => x.Name == propertyName); 
            if (property == null || !json.ContainsKey(propertyName))
            {
                logger.LogInformation($"Put Property: Property not found [[thing: {thingId}][property: {propertyName}]]");
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            property.Value = json[propertyName];
            

            var writer = services.GetRequiredService<IHttpBodyWriter>();
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await writer.WriteAsync(new Dictionary<string, object>
            {
                [propertyName] = property.Value
            }, httpContext.RequestAborted);
        }
    }
}
