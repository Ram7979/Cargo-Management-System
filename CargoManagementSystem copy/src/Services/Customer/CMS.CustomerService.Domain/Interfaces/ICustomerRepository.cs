using CMS.CustomerService.Domain.Entities;

namespace CMS.CustomerService.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByCustomerCodeAsync(string customerCode);
    Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? search = null,
        string? type = null,
        bool? isActive = null);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task<bool> ExistsAsync(string email);
    Task<int> GetNextSequenceAsync(int year);
}
