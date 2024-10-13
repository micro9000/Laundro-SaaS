using Laundro.API.Authentication;
using Laundro.API.Authorization;
using Laundro.API.Infrastructure.Exceptions;
using Laundro.API.Plumbing;
using Laundro.API.Plumbing.Database;
using Laundro.Core.BusinessRequirementsValidators;
using Serilog;

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
    builder.Services.AddDatabaseStorage(builder.Configuration);
    builder.Services.AddCaching(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddLaundroAzureADAuthentication(builder.Configuration);
    builder.Services.AddLaundroAuthorization(builder.Configuration);

    builder.Services.AddBusinessRequirementsValidators();

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


