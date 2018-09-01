using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.IdGenerators;
namespace Esmart.Framework.DB
{
    public static class MongodbExtension
    {
        public static IMongoQuery ConvertQuery(this string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return new QueryDocument();
            BsonDocument queryDoc = BsonSerializer.Deserialize<BsonDocument>(queryString);
            QueryDocument query = new QueryDocument(queryDoc);
            return query;
        }

        public static UpdateDocument ConvertUpdate(this string updateString)
        {
            BsonDocument updateDoc = BsonSerializer.Deserialize<BsonDocument>(updateString);
            UpdateDocument update = new UpdateDocument(updateDoc);
            return update;
        }

    }
}
