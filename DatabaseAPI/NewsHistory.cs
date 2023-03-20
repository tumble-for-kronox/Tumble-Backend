using MongoDB.Driver;
using WebAPIModels.MiscModels;
using WebAPIModels.ResponseModels;

namespace DatabaseAPI;

public class NewsHistory
{
    public static async Task<List<NotificationContent>> GetNewsHistory()
    {
        if (Connector.News == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        List<NotificationContent> cursor = await Connector.News.Find(Builders<NotificationContent>.Filter.Empty).ToListAsync();

        if (cursor.Any())
            return cursor;

        return new();
    }

    public static void SaveNewsItem(NotificationContent newsNotificationContent)
    {
        if (Connector.News == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        Connector.News.InsertOne(newsNotificationContent);
    }
}
