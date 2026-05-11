using Azure.Storage.Blobs;
using CMS.ReportingService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CMS.ReportingService.Infrastructure.Services;

public class AzureBlobService : IBlobService
{
    private readonly string _connectionString;

    public AzureBlobService(IConfiguration configuration)
    {
        _connectionString = configuration["AzureStorage:ConnectionString"] ?? string.Empty;
    }

    public async Task<string> UploadAsync(string containerName, string fileName, byte[] content)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            return $"https://localhost/blob/{containerName}/{fileName}";
        }

        var serviceClient = new BlobServiceClient(_connectionString);
        var containerClient = serviceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);
        using var stream = new MemoryStream(content);
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }
}
