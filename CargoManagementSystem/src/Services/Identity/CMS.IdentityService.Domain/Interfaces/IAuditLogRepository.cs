using CMS.IdentityService.Domain.Entities;

namespace CMS.IdentityService.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<IEnumerable<AuditLog>> GetByActorAsync(string actorId, int page, int pageSize);
}
