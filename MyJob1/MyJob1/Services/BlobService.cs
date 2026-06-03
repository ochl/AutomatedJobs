using Azure.Identity;
using Azure.Storage.Blobs;

public class BlobService
{
    private readonly BlobServiceClient _client;

    public BlobService()
    {
        _client = new BlobServiceClient(
            new Uri("https://ochitplstorage.blob.core.windows.net"),
            new DefaultAzureCredential());
    }

    public async IAsyncEnumerable<string> GetBlobsAsync(string container)
    {
        var containerClient = _client.GetBlobContainerClient(container);

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            yield return blob.Name;
        }
    }

    public async Task<string> ReadBlobAsync(string container, string name)
    {
        var containerClient = _client.GetBlobContainerClient(container);
        var blob = containerClient.GetBlobClient(name);

        var response = await blob.DownloadContentAsync();
        return response.Value.Content.ToString();
    }

    public async Task WriteBlobAsync(string container, string name, string content)
    {
        var containerClient = _client.GetBlobContainerClient(container);
        var blob = containerClient.GetBlobClient(name);

        await blob.UploadAsync(BinaryData.FromString(content), overwrite: true);
    }
}