using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyJob1.Interfaces;
using MyJob1.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(new BlobServiceClient(
    new Uri("https://ochitplstorage.blob.core.windows.net"),
    new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        AdditionallyAllowedTenants = { "*" }
    })));

builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<KeyVaultService>();
builder.Services.AddSingleton<JobWorker>();

var host = builder.Build();

var worker = host.Services.GetRequiredService<JobWorker>();
await worker.RunAsync();