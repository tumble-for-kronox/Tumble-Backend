using MongoDB.Driver;
using WebAPIModels.ResponseModels;

namespace DatabaseAPI;

/// <summary>
/// .
/// </summary>
public static class SchedulesCache
{
    public static ScheduleWebModel? GetSchedule(string id)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        List<ScheduleWebModel> cursor = Connector.Schedules.Find(Builders<ScheduleWebModel>.Filter.Eq("_id", id)).ToList();

        if (cursor.Any())
            return cursor.First();

        return null;
    }

    public static void SaveSchedule(ScheduleWebModel schedule)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        Connector.Schedules.InsertOne(schedule);
    }

    public static void UpdateSchedule(string id, ScheduleWebModel schedule)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        Connector.Schedules.ReplaceOne(Builders<ScheduleWebModel>.Filter.Eq("_id", id), schedule);
    }
}
