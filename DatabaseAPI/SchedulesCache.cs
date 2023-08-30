using MongoDB.Driver;
using WebAPIModels.ResponseModels;

namespace DatabaseAPI;

/// <summary>
/// .
/// </summary>
public static class SchedulesCache
{
    public static async Task<ScheduleWebModel?> GetSchedule(string id)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        List<ScheduleWebModel> cursor = await Connector.Schedules.Find(Builders<ScheduleWebModel>.Filter.Eq("_id", id)).ToListAsync();

        if (cursor.Any())
            return cursor.First();

        return null;
    }

    public static async Task SaveSchedule(ScheduleWebModel schedule)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        await Connector.Schedules.InsertOneAsync(schedule);
    }

    public static async Task UpdateSchedule(string id, ScheduleWebModel schedule)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        await Connector.Schedules.ReplaceOneAsync(Builders<ScheduleWebModel>.Filter.Eq("_id", id), schedule);
    }

    /// <summary>
    ///  This operation will insert a new document if it doesn't exist, or update the existing one if it does
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns></returns>
    public static async Task UpsertSchedule(ScheduleWebModel schedule)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        var filter = Builders<ScheduleWebModel>.Filter.Eq("Id", schedule.Id);

        var options = new ReplaceOptions { IsUpsert = true };
        await Connector.Schedules.ReplaceOneAsync(filter, schedule, options);
    }

}
