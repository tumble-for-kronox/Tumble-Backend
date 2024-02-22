using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling;

public class Location
{
    public Location(string id, string name, string building, string floor, int maxSeats)
    {
        Id = id;
        Name = name;
        Building = building;
        Floor = floor;
        MaxSeats = maxSeats;
    }

    /// <summary>
    /// For use as default or in case a location is not found.
    /// </summary>
    /// <returns><see cref="Location"/> wiht all values set as "N/A".</returns>
    public static Location NotAvailable => new("N/A", "N/A", "N/A", "N/A", 0);

    public override string? ToString()
    {
        return $"{Id}";
    }

    public string Id { get; }

    public string Name { get; }

    public string Building { get; }

    public string Floor { get; }

    public int MaxSeats { get; }
}
