using CMS.Shared.Entities;

namespace CMS.CustomerService.Domain.Entities;

public class KycDocument : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string BlobReference { get; private set; } = string.Empty;
    public DateTime UploadedAt { get; private set; }

    private KycDocument() { }

    public static KycDocument Create(Guid customerId, string documentType, string blobReference)
    {
        return new KycDocument
        {
            CustomerId = customerId,
            DocumentType = documentType,
            BlobReference = blobReference,
            UploadedAt = DateTime.UtcNow
        };
    }
}
