using System;
using MongoDB.Driver;

namespace kiki.shared {
    public class KiKiDBContext {
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Community> communities;
        private readonly IMongoCollection<Post> posts;

        public KiKiDBContext() {
            var client = new MongoClient("mongo://127.0.0.1:27017");
            db = client.GetDatabase("KiKiDB");
            communities = db.GetCollection<Community>("Communities");
            posts = db.GetCollection<Post>("Posts");
        }
    }
}