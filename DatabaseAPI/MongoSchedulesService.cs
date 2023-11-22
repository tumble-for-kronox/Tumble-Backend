using DatabaseAPI.Interfaces;
using MongoDB.Driver;
using WebAPIModels.ResponseModels;

namespace DatabaseAPI;

public class MongoSchedulesService : IDbSchedulesService
{
    private readonly IMongoCollection<ScheduleWebModel> _scheduleCollection;

    public MongoSchedulesService(IDbSettings settings)
    {
        MongoClient client = new(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
        _scheduleCollection = database.GetCollection<ScheduleWebModel>("schedules");
    }

    public async Task<ScheduleWebModel?> GetScheduleAsync(string id)
    {
        List<ScheduleWebModel> scheduleItems = await _scheduleCollection.Find(Builders<ScheduleWebModel>.Filter.Eq("_id", id)).Limit(1).ToListAsync();

        if (scheduleItems.Count == 0)
        {
            return null;
        }

        return scheduleItems.First();
    }

    public async Task UpsertScheduleAsync(ScheduleWebModel schedule)
    {
        var filter = Builders<ScheduleWebModel>.Filter.Eq("_id", schedule.Id);

        var options = new ReplaceOptions { IsUpsert = true };
        await _scheduleCollection.ReplaceOneAsync(filter, schedule, options);
    }
}
