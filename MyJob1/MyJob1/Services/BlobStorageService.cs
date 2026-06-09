using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyJob1.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyJob1.Services
{
    public sealed class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IAppLogger<BlobStorageService> _logger;

        public BlobStorageService(BlobServiceClient blobServiceClient, IAppLogger<BlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async IAsyncEnumerable<BlobItem> ListBlobsAsync(string containerName, string? folderPrefix = null, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            string prefix = string.IsNullOrWhiteSpace(folderPrefix)
                ? string.Empty
                : folderPrefix.TrimEnd('/') + "/";

            await foreach (var blob in container.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellationToken))
            {
                yield return blob;
            }
        }

        public async Task<Stream> OpenReadAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            return await container.GetBlobClient(blobName).OpenReadAsync(cancellationToken: cancellationToken);
        }

        public async Task UploadAsync(string containerName, string blobName, Stream content, bool overwrite = true, CancellationToken cancellationToken = default)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            await container.GetBlobClient(blobName).UploadAsync(content, overwrite, cancellationToken);
        }

        public async Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            await container.GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public async Task MoveBlobAsync(string containerName, string sourceBlobName, string targetBlobName, CancellationToken cancellationToken = default)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            var sourceBlob = container.GetBlobClient(sourceBlobName);
            var targetBlob = container.GetBlobClient(targetBlobName);

            var operation = await targetBlob.StartCopyFromUriAsync(sourceBlob.Uri, cancellationToken: cancellationToken);

            await operation.WaitForCompletionAsync(cancellationToken);

            await sourceBlob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        public async Task ProcessFolderAsync(string containerName, string folderPrefix, Func<BlobItem, Stream, Task> processor, int maxConcurrency = 3, CancellationToken cancellationToken = default)
        {
            _logger.Info("ProcessFolderAsync {containerName}, {folderPrefix}", containerName, folderPrefix);

            var container = _blobServiceClient.GetBlobContainerClient(containerName);

            string prefix = string.IsNullOrWhiteSpace(folderPrefix) ? string.Empty : folderPrefix.TrimEnd('/') + "/";

            await Parallel.ForEachAsync(container.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellationToken),
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = maxConcurrency,
                    CancellationToken = cancellationToken
                },
                async (blobItem, ct) =>
                {
                    if (blobItem.Name.EndsWith("/"))
                        return;

                    var blobClient = container.GetBlobClient(blobItem.Name);

                    await using var stream = await blobClient.OpenReadAsync(cancellationToken: ct);

                    await processor(blobItem, stream);
                });
        }
    }
}
