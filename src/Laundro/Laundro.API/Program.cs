using Laundro.API.Authentication;
using Laundro.API.Data;
using Laundro.API.Plumbing;
using Microsoft.IdentityModel.Logging;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Serilog;
using System.Text.Json;

Log.Logger = ConfigureSerilogLogging.BootstrapLogger;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.AddSerilogLogging(builder.Configuration);
    builder.AddServiceDefaults();

    // We can remove this if we are not going to use controllers later
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
        });

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddGlobalCorsPolicy(builder.Configuration);
    builder.Services.AddCustomNodaTimeClock();

    // Application components
    builder.Services.AddDatabaseStorage(builder.Configuration);
    builder.Services.AddCaching(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.AddLaundroAzureADAuthentication(builder.Configuration);


    var app = builder.Build();
    app.UseSerilogLogging();
    app.MapDefaultEndpoints();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        IdentityModelEventSource.ShowPII = true;
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.UseHttpsRedirection();

    app.UseGlobalCorsPolicy();
    app.UseLaundroAzureADAuthentication();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


