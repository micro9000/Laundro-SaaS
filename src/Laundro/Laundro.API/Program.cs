using Laundro.API.Authentication;
using Laundro.API.Data;

var builder = WebApplication.CreateBuilder(args);

// We can remove this if we are not going to use controllers later
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLaundroAzureADAuthentication(builder.Configuration);
builder.Services.AddGlobalCorsPolicy(builder.Configuration);

// Application components
builder.Services.AddDatabaseStorage(builder.Configuration);
builder.Services.AddCaching(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
//app.UseHttpsRedirection();

app.UseGlobalCorsPolicy();
app.UseLaundroAzureADAuthentication();

app.MapControllers();

app.Run();
