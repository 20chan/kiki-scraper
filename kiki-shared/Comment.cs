using System;
using MongoDB.Bson.Serialization.Attributes;

namespace kiki.shared {
    public class Comment {
        [BsonId]
        public int Id { get; set; }
        public string Author { get; set; }
        public DateTime WrittenDate { get; set; }
        public string Content { get; set; }
    }
}