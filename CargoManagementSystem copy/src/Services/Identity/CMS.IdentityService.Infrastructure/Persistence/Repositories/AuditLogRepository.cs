using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.IdentityService.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly IdentityDbContext _context;

    public AuditLogRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByActorAsync(string actorId, int page, int pageSize)
    {
        return await _context.AuditLogs
            .Where(a => a.ActorId == actorId)
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
