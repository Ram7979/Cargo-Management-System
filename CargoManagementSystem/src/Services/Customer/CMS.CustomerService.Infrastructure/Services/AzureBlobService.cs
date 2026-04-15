using Azure.Storage.Blobs;
using CMS.CustomerService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CMS.CustomerService.Infrastructure.Services;

public class AzureBlobService : IBlobService
{
    private readonly string _connectionString;
    private readonly ILogger<AzureBlobService> _logger;

    public AzureBlobService(IConfiguration configuration, ILogger<AzureBlobService> logger)
    {
        _connectionString = configuration["AzureStorage:ConnectionString"] ?? string.Empty;
        _logger = logger;
    }

    public async Task<string> UploadAsync(string containerName, string fileName, byte[] content, string contentType = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            _logger.LogInformation("Azure Storage not configured. Returning stub URL for {FileName}", fileName);
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

    public async Task DeleteAsync(string containerName, string blobName)
    {
        if (string.IsNullOrWhiteSpace(_connectionString)) return;

        var serviceClient = new BlobServiceClient(_connectionString);
        var containerClient = serviceClient.GetBlobContainerClient(containerName);
        await containerClient.DeleteBlobIfExistsAsync(blobName);
    }
}
