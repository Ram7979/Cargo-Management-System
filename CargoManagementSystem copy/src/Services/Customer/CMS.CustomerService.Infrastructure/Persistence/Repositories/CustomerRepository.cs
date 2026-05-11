using CMS.CustomerService.Domain.Entities;
using CMS.CustomerService.Domain.Enums;
using CMS.CustomerService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.CustomerService.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _context;

    public CustomerRepository(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
        => await _context.Customers.Include(c => c.KycDocuments).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Customer?> GetByEmailAsync(string email)
        => await _context.Customers.Include(c => c.KycDocuments).FirstOrDefaultAsync(c => c.Email == email);

    public async Task<Customer?> GetByCustomerCodeAsync(string customerCode)
        => await _context.Customers.Include(c => c.KycDocuments).FirstOrDefaultAsync(c => c.CustomerCode == customerCode);

    public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search = null, string? type = null, bool? isActive = null)
    {
        var query = _context.Customers.Include(c => c.KycDocuments).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.FullName.Contains(search) ||
                c.Email.Contains(search) ||
                c.CustomerCode.Contains(search) ||
                c.CompanyName.Contains(search));

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<CustomerType>(type, ignoreCase: true, out var ct))
            query = query.Where(c => c.Type == ct);

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
        => await _context.Customers.Include(c => c.KycDocuments).OrderByDescending(c => c.CreatedAt).ToListAsync();

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string email)
        => await _context.Customers.AnyAsync(c => c.Email == email);

    public async Task<int> GetNextSequenceAsync(int year)
    {
        var count = await _context.Customers.CountAsync(c => c.CreatedAt.Year == year);
        return count + 1;
    }
}
