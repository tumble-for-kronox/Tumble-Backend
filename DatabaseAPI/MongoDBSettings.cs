using DatabaseAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAPI;

public class MongoDBSettings : IDbSettings
{
    public MongoDBSettings(string connectionString, string databaseName)
    {
        ConnectionString = connectionString;
        DatabaseName = databaseName;
    }

    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}
