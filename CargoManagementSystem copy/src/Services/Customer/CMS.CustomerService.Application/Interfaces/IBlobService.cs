namespace CMS.CustomerService.Application.Interfaces;

public interface IBlobService
{
    Task<string> UploadAsync(string containerName, string fileName, byte[] content, string contentType = "application/octet-stream");
    Task DeleteAsync(string containerName, string blobName);
}
