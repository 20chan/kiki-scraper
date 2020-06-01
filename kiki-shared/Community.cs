using MongoDB.Bson;

namespace kiki.shared {
    public class Community {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}