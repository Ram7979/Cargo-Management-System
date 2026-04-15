namespace CMS.CustomerService.Application.DTOs;

public class KycDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string BlobReference { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
