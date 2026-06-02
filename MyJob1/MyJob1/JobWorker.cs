
public class JobWorker
{
    private readonly BlobService _blobService;
    private readonly KeyVaultService _keyVault;

    public JobWorker(BlobService blobService, KeyVaultService keyVault)
    {
        _blobService = blobService;
        _keyVault = keyVault;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Job started...");

        // Optional: fetch secret
        var apiKey = await _keyVault.GetSecretAsync("api-key");

        // Process blobs
        await foreach (var blob in _blobService.GetBlobsAsync("input-container"))
        {
            Console.WriteLine($"Processing: {blob}");

            // Example processing logic
            var content = await _blobService.ReadBlobAsync("input-container", blob);

            var result = content.ToUpperInvariant();

            await _blobService.WriteBlobAsync("output-container", blob, result);
        }

        Console.WriteLine("Job completed.");
    }
}