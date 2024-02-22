using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Users;

/// <summary>
/// Model for storing and managing UserEvents (e.g. exams) in Kronox's database.
/// </summary>
public abstract class UserEvent
{
    public string Title { get; }

    public string Type { get; }

    public DateTime EventStart { get; }

    public DateTime EventEnd { get; }

    protected UserEvent(string title, string type, DateTime eventStart, DateTime eventEnd)
    {
        Title = title;
        Type = type;
        EventStart = eventStart;
        EventEnd = eventEnd;
    }
}
