using CMS.Shared.Entities;

namespace CMS.WarehouseService.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public ICollection<Bin> Bins { get; private set; } = new List<Bin>();

    private Warehouse() { }

    public static Warehouse Create(string name, string address, string city, string country)
    {
        return new Warehouse
        {
            Name = name,
            Address = address,
            City = city,
            Country = country
        };
    }

    public Bin? GetAvailableBin()
        => Bins.FirstOrDefault(b => !b.IsOccupied);
}
