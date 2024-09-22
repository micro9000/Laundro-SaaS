var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Laundro_API>("laundro-api");

builder.Build().Run();
