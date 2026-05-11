using CMS.CustomerService.Domain.Entities;
using CMS.CustomerService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.CustomerService.Infrastructure.Persistence.Repositories;

public class KycDocumentRepository : IKycDocumentRepository
{
    private readonly CustomerDbContext _context;

    public KycDocumentRepository(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<KycDocument?> GetByIdAsync(Guid id)
        => await _context.KycDocuments.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IEnumerable<KycDocument>> GetByCustomerIdAsync(Guid customerId)
        => await _context.KycDocuments.Where(d => d.CustomerId == customerId)
            .OrderByDescending(d => d.UploadedAt).ToListAsync();

    public async Task AddAsync(KycDocument document)
    {
        await _context.KycDocuments.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var doc = await _context.KycDocuments.FindAsync(id);
        if (doc != null)
        {
            _context.KycDocuments.Remove(doc);
            await _context.SaveChangesAsync();
        }
    }
}
