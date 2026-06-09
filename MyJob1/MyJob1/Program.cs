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

Log.Information("Application started "+DateTime.Now);

Log.CloseAndFlush();


builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<KeyVaultService>();
builder.Services.AddSingleton<JobWorker>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();



var host = builder.Build();

Console.WriteLine(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING"));


var worker = host.Services.GetRequiredService<JobWorker>();
await worker.RunAsync();