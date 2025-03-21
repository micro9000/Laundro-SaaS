using Laundro.API.Authentication;
using Laundro.API.Authorization;
using Laundro.API.Infrastructure.Exceptions;
using Laundro.API.Plumbing;
using Laundro.Core.BusinessRequirementsValidators;
using Serilog;
using Laundro.API;
using Laundro.API.Storage;
using Laundro.API.Database;
using System.Text.Json.Serialization;
using Laundro.Core.Utilities;

Log.Logger = ConfigureSerilogLogging.BootstrapLogger;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.AddSerilogLogging(builder.Configuration);
    builder.AddServiceDefaults();

    // Exception handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddFastEndpointsConfigs();
    builder.Services.AddGlobalCorsPolicy(builder.Configuration);
    builder.Services.AddCustomNodaTimeClock();

    // Application components
    builder.Services.AddScoped<IIdObfuscator, IdObfuscator>();
    builder.Services.AddDatabaseStorage(builder.Configuration);
    builder.Services.AddCaching(builder.Configuration);
    builder.Services.AddBlobStorage(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddLaundroAzureADAuthentication(builder.Configuration);
    builder.Services.AddLaundroAuthorization(builder.Configuration);

    builder.Services.AddBusinessRequirementsValidators();

    builder.Services.AddHostedService<StartupRunner>();
    builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options 
        => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    var app = builder.Build();
    app.UseSerilogLogging();
    app.UseExceptionHandler();

    // health checks endpoints
    app.MapDefaultEndpoints();
    app.UseRouting();
    app.UseHttpsRedirection();

    app.UseGlobalCorsPolicy();
    app.UseLaundroAzureADAuthentication();

    app.UseFastEndpointsConfigs();
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}


