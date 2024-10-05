﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Laundro.Core.Authentication;

namespace Laundro.API.Authentication;

public static class AuthenticationRegistration
{
    public static IServiceCollection AddLaundroAzureADAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

        services.AddUserAccountStateServices();

        return services;
    }

    public static IApplicationBuilder UseLaundroAzureADAuthentication(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseMiddleware<UserContextMiddleware>(); // Order matters here as this caches the user context used by the auth handlers
        app.UseAuthorization();
        return app;
    }
}
