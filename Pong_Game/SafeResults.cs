using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver; 

namespace Pong_Game
{
    internal class SaveData
    {
        static SaveData()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("PongDB");
        }

    }
}
