
using Azure.Core.Diagnostics;
using Azure.Identity;
using Azure.Storage.Blobs;
using MyJob1.Interfaces;
using MyJob1.Services;

public class JobWorker
{
    private readonly KeyVaultService _keyVault;
    private readonly IBlobStorageService _blobStorageService;

    public JobWorker(KeyVaultService keyVault, IBlobStorageService blobStorageService)
    {
        _keyVault = keyVault;
        _blobStorageService = blobStorageService;
        AzureEventSourceListener.CreateConsoleLogger();
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Job started...");

        // Optional: fetch secret
        var apiKey = await _keyVault.GetSecretAsync("api-key");

        await _blobStorageService.ProcessFolderAsync(
            containerName: "ochitstoragecontainer",
            folderPrefix: "/OpenEir/",
            processor: async (blob, stream) =>
            {   
                using var reader = new StreamReader(stream);

                var content = await reader.ReadToEndAsync();

                Console.WriteLine($"Processing {blob.Name}");
            }, 
            maxConcurrency:1);

        Console.WriteLine("Job completed.");
    }
}