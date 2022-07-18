using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPIModels;

namespace DatabaseAPI;

public static class SchedulesCollection
{
    public static ScheduleWebModel GetSchedule(string id)
    {
        if (Connector.Schedules == null) throw new Exceptions.DatabaseUninitializedException("The database/collection is not initialized. Run .Init() on the Connector before attempting to access the database.");

        return Connector.Schedules.FindSync<ScheduleWebModel>(Builders<ScheduleWebModel>.Filter.Eq("_id", id)).Single();
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
