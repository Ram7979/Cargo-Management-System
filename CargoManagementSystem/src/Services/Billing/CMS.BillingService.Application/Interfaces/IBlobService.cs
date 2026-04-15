namespace CMS.BillingService.Application.Interfaces;

public interface IBlobService
{
    Task<string> UploadAsync(string containerName, string fileName, byte[] content);
}
