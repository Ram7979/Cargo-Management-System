using System.Reflection;
using CMS.BillingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.BillingService.Infrastructure.Persistence;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<RateCard> RateCards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
