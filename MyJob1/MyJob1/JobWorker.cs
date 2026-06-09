
using Azure.Core.Diagnostics;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using MyJob1.Interfaces;
using MyJob1.Services;

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
            _logger.LogError("Job started -------->");
            _logger.LogInformation("Application started");
            _logger.LogWarning("This is a warning");
            _logger.LogError("This is an error");




            // Give the background sender time to transmit
            await Task.Delay(TimeSpan.FromSeconds(5));


            Console.WriteLine("Job started...");

            // Optional: fetch secret
            var apiKey = await _keyVault.GetSecretAsync("api-key");

            await _blobStorageService.ProcessFolderAsync(
                containerName: "ochitstoragecontainer",
                folderPrefix: "/OpenEir/In/UGNOT/",
                processor: async (blob, stream) =>
                {
                    using var reader = new StreamReader(stream);

                    var content = await reader.ReadToEndAsync();

                    Console.WriteLine($"Processing {blob.Name}");
                },
                maxConcurrency: 1);

            Console.WriteLine("Job completed.");
        }
        catch
        {

        }
        finally 
        {
            _logger.LogError("Job finished");

         
            await Task.Delay(5000);
        }
    }
}