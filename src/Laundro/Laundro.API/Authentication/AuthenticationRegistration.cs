using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace Laundro.API.Authentication;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddLaundroAzureADAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration);

        return services;
    }

    public static IApplicationBuilder UseLaundroAzureADAuthentication(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        // Middlewares
        app.UseAuthorization();
        return app;
    }
}
