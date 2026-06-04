using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyJob1.Interfaces
{
    public interface IBlobStorageService
    {
        IAsyncEnumerable<BlobItem> ListBlobsAsync(string containerName, string? folderPrefix = null, CancellationToken cancellationToken = default);

        Task<Stream> OpenReadAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

        Task MoveBlobAsync(string containerName, string sourceBlobName, string targetBlobName, CancellationToken cancellationToken = default);

        Task UploadAsync(string containerName, string blobName, Stream content, bool overwrite = true, CancellationToken cancellationToken = default);

        Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

        Task ProcessFolderAsync(string containerName, string folderPrefix, Func<BlobItem, Stream, Task> processor, int maxConcurrency = 3, CancellationToken cancellationToken = default);
    }
}
