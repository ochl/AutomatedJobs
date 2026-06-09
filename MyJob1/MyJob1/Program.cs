using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJob1.Interfaces;
using MyJob1.Services;

using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(new BlobServiceClient(
    new Uri("https://ochitplstorage.blob.core.windows.net"),
    new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        AdditionallyAllowedTenants = { "*" }
    })));


var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
telemetryConfiguration.ConnectionString = connectionString;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

Log.Information("Application started 4 "+DateTime.Now);


builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<KeyVaultService>();
builder.Services.AddSingleton<JobWorker>();


var host = builder.Build();

var worker = host.Services.GetRequiredService<JobWorker>();
await worker.RunAsync();