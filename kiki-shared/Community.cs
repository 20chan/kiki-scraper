using MongoDB.Bson.Serialization.Attributes;

namespace kiki.shared {
    public class Community {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}