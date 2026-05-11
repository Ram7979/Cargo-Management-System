using CMS.CustomerService.Domain.Entities;

namespace CMS.CustomerService.Domain.Interfaces;

public interface IKycDocumentRepository
{
    Task<KycDocument?> GetByIdAsync(Guid id);
    Task<IEnumerable<KycDocument>> GetByCustomerIdAsync(Guid customerId);
    Task AddAsync(KycDocument document);
    Task DeleteAsync(Guid id);
}
