namespace KronoxAPI.Model.Scheduling;

public class Teacher
{
    public Teacher(string id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>
    /// For use as default or in case a teacher is not found, to make sure nothing breaks.
    /// </summary>
    /// <returns><see cref="Teacher"/> with all values set as "N/A"</returns>
    public static Teacher NotAvailable => new("N/A", "N/A", "N/A");

    public override string? ToString()
    {
        return $"{FirstName} {LastName}";
    }

    public string Id { get; }

    public string FirstName { get; }

    public string LastName { get; }
}
