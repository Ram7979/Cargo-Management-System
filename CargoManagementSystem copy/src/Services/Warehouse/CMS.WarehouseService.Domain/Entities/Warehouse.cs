using CMS.Shared.Entities;

namespace CMS.WarehouseService.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public decimal CapacityKg { get; private set; }
    public int TotalBins { get; private set; }

    private readonly List<Bin> _bins = new();
    public ICollection<Bin> Bins => _bins.AsReadOnly();

    private readonly List<CargoReceipt> _receipts = new();
    public ICollection<CargoReceipt> Receipts => _receipts.AsReadOnly();

    private Warehouse() { }

    public static Warehouse Create(string name, string address, string city, string country,
        decimal capacityKg = 0, int totalBins = 0)
    {
        return new Warehouse
        {
            Name = name,
            Address = address,
            City = city,
            Country = country,
            CapacityKg = capacityKg,
            TotalBins = totalBins
        };
    }

    public void Update(string? name, string? address, string? city, string? country, decimal? capacityKg)
    {
        if (name != null) Name = name;
        if (address != null) Address = address;
        if (city != null) City = city;
        if (country != null) Country = country;
        if (capacityKg.HasValue) CapacityKg = capacityKg.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public Bin? GetAvailableBin(decimal requiredWeightKg = 0)
        => _bins.FirstOrDefault(b => !b.IsOccupied && b.IsActive && b.CapacityKg >= requiredWeightKg);
}
