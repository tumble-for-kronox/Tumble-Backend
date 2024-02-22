using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling;

/// <summary>
/// Model for storing information relevant to a given course on Kronox.
/// </summary>
public class Course
{
    public string Name { get; }

    public string Id { get; }

    /// <summary>
    /// For use as default or in case a course is not found.
    /// </summary>
    /// <returns><see cref="Course"/> wiht all values set as "N/A".</returns>
    public static Course NotAvailable => new("N/A", "N/A");

    public Course(string name, string id)
    {
        Name = name;
        Id = id;
    }
}
