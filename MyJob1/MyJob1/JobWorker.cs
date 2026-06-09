
using Azure.Core.Diagnostics;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using MyJob1.Interfaces;
using MyJob1.Services;
using Serilog;
using System.Reflection.Metadata;
using static System.Reflection.Metadata.BlobBuilder;

public class JobWorker
{
    private readonly KeyVaultService _keyVault;
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<JobWorker> _logger;

    public JobWorker(KeyVaultService keyVault, IBlobStorageService blobStorageService, ILogger<JobWorker> logger)
    {
        _keyVault = keyVault;
        _blobStorageService = blobStorageService;
        _logger = logger;
         AzureEventSourceListener.CreateConsoleLogger();
    }

    public async Task RunAsync()
    {
        try
        {
            _logger.LogInformation("JobWorker started 4");
            

            // Optional: fetch secret
            var apiKey = await _keyVault.GetSecretAsync("api-key");

            await _blobStorageService.ProcessFolderAsync(
                containerName: "ochitstoragecontainer",
                folderPrefix: "/OpenEir/In/UGNOT/",
                processor: async (blob, stream) =>
                {
                    using var reader = new StreamReader(stream);

                    var content = await reader.ReadToEndAsync();

                    Log.Information($"Processing {blob.Name}");
                    Console.WriteLine($"Processing {blob.Name}");
                },
                maxConcurrency: 1);

            Console.WriteLine("Job completed.");
        }
        catch(Exception ex)
        {
            Log.Information($"Processing {ex.ToString()}");
        }
        finally 
        {
            Log.Information("JobWorker finished 4");
            Log.CloseAndFlush();
        }
    }
}