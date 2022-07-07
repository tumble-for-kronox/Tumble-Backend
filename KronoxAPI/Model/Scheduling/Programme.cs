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
    private readonly string _title;
    private readonly string _subtitle;
    private readonly string _id;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="subtitle"></param>
    /// <param name="id"></param>
    public Programme(string title, string subtitle, string id)
    {
        _title = title;
        _subtitle = subtitle;
        _id = id;
    }

    /// <summary>
    /// For use as default or in case a programme is not found.
    /// </summary>
    /// <returns><see cref="Programme"/> with all values set as "N/A"</returns>
    public static Programme NotAvailable => new("N/A", "N/A", "N/A");

    public string Title => _title;

    public string Subtitle => _subtitle;

    public string Id => _id;
}
