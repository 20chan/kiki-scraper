using System;
using MongoDB.Driver;

namespace kiki.shared {
    public class KiKiDBContext {
        private readonly IMongoDatabase db;

        public KiKiDBContext() {
            var client = new MongoClient("mongo://127.0.0.1:27017");
            db = client.GetDatabase("KiKiDB");
        }
    }
}