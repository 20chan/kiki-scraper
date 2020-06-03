using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kiki.shared {
    public class Community {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}