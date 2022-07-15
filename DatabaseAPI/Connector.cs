using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DatabaseAPI
{
    public class Connector
    {
        public readonly MongoClient client = new MongoClient();
    }
}
