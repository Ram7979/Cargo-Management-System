using System.Reflection;
using CMS.WarehouseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMS.WarehouseService.Infrastructure.Persistence;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Warehouse> Warehouses { get; set; } = null!;
    public DbSet<Bin> Bins { get; set; } = null!;
    public DbSet<CargoReceipt> CargoReceipts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
