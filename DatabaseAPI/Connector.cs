using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using WebAPIModels.MiscModels;
using WebAPIModels.ResponseModels;

namespace DatabaseAPI;

public static class Connector
{
    private static MongoClient? _client;
    private static MongoDatabaseBase? _database;
    private static MongoCollectionBase<ScheduleWebModel>? _schedules;
    private static MongoCollectionBase<SchoolProgrammeFilter>? _programmeFilters;

    public static void Init(string connectionString)
    {
        var conventions = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };

        ConventionRegistry.Register("Custom Conventions", conventions, t => true);

        _client = new MongoClient(connectionString);
        _database = (MongoDatabaseBase)_client.GetDatabase("test_db");
        _schedules = (MongoCollectionBase<ScheduleWebModel>)_database.GetCollection<ScheduleWebModel>("schedules");
        _programmeFilters = (MongoCollectionBase<SchoolProgrammeFilter>)_database.GetCollection<SchoolProgrammeFilter>("programme_filters");

    }

    internal static MongoCollectionBase<ScheduleWebModel>? Schedules => _schedules;
    internal static MongoCollectionBase<SchoolProgrammeFilter>? ProgrammeFilters => _programmeFilters;
}
