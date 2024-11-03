using FastEndpoints;
using FastEndpoints.Swagger;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Text.Json;

namespace Laundro.API.Plumbing;

public static class FastEndpointsRegistration
{
    public static IServiceCollection AddFastEndpointsConfigs(
        this IServiceCollection services)
    {
        services.AddFastEndpoints()
            .SwaggerDocument(o =>
            {
                o.DocumentSettings = s =>
                {
                    s.Title = "Laundro API";
                    s.Version = "v1";
                };
            });
        return services;
    }

    public static IApplicationBuilder UseFastEndpointsConfigs(this IApplicationBuilder app)
    {
        //app.UseDefaultExceptionHandler(); // Let's use the global exception handlers in Infrastructure dir for now
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
            c.Serializer.Options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            c.Serializer.Options.WriteIndented = true;
        }).UseSwaggerGen();

        return app;
    }
}
