using FastEndpoints;

namespace Laundro.API.Features;

public static class EndpointExtensions
{
    public static Task SendStatusCode(this IEndpoint ep, int statusCode, CancellationToken ct = default)
    {
        ep.HttpContext.MarkResponseStart(); //don't forget to always do this
        ep.HttpContext.Response.StatusCode = statusCode;
        return ep.HttpContext.Response.StartAsync(ct);
    }
}
