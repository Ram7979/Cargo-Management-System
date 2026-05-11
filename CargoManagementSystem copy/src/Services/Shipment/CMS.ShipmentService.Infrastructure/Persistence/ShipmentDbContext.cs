using System.Reflection;
using CMS.ShipmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.ShipmentService.Infrastructure.Persistence;

public class ShipmentDbContext : DbContext
{
    public ShipmentDbContext(DbContextOptions<ShipmentDbContext> options) : base(options) { }

    public DbSet<Shipment> Shipments { get; set; } = null!;
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
