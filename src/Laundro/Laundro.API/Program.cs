using Laundro.API.Authentication;
using Laundro.API.Data;
using Laundro.API.Infrastructure.Exceptions;
using Laundro.API.Plumbing;
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


