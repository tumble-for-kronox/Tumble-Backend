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
    private readonly string _title;
    private readonly string _type;
    private readonly DateTime _eventStart;
    private readonly DateTime _eventEnd;
    public string Title => _title;

    public string Type => _type;

    public DateTime EventStart => _eventStart;

    public DateTime EventEnd => _eventEnd;

    public UserEvent(string title, string type, DateTime eventStart, DateTime eventEnd)
    {
        _title = title;
        _type = type;
        _eventStart = eventStart;
        _eventEnd = eventEnd;
    }
}
