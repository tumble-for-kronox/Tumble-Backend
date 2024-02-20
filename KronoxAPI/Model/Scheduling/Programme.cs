using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling;

/// <summary>
/// Model for programme data found through Kronox's database.
/// </summary>
public class Programme
{
    public Programme(string title, string subtitle, string id)
    {
        Title = title;
        Subtitle = subtitle;
        Id = id;
    }

    /// <summary>
    /// For use as default or in case a programme is not found.
    /// </summary>
    /// <returns><see cref="Programme"/> with all values set as "N/A"</returns>
    public static Programme NotAvailable => new("N/A", "N/A", "N/A");

    public string Title { get; }

    public string Subtitle { get; }

    public string Id { get; }
}
