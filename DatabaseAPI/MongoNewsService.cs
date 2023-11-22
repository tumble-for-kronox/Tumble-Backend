using DatabaseAPI.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using WebAPIModels.MiscModels;

namespace DatabaseAPI;

public class MongoNewsService : IDbNewsService
{
    private readonly IMongoCollection<NotificationContent> _newsCollection;

    public MongoNewsService(IDbSettings settings)
    {
        MongoClient client = new(settings.ConnectionString);
        IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
        _newsCollection = database.GetCollection<NotificationContent>("news_history");
    }

    public async Task<List<NotificationContent>> GetNewsHistoryAsync()
    {
        List<NotificationContent> cursor = await _newsCollection.AsQueryable().OrderByDescending(c => c.Timestamp).Take(10).ToListAsync();

        if (cursor.Any())
            return cursor;

        return new();
    }

    public async Task SaveNewsItemAsync(NotificationContent item)
    {
        await _newsCollection.InsertOneAsync(item);
    }
}
