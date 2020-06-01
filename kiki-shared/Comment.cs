using System;
using MongoDB.Bson;

namespace kiki.shared {
    public class Comment {
        public ObjectId Id { get; set; }
        public string Author { get; set; }
        public DateTime WrittenDate { get; set; }
        public string Content { get; set; }
    }
}