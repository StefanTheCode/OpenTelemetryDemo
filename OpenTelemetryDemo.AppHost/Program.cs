var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.OpenTelemetryDemo>("NewsletterApi").WithExternalHttpEndpoints();

builder.Build().Run();
