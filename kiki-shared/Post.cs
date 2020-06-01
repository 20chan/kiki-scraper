using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kiki.shared {
    public class Post {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId Community { get; set; }
        public DateTime ScrappedDate { get; set; }

        public string PostId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime WrittenDate { get; set; }
        public string Content { get; set; }

        public Comment[] Comments { get; set; }
    }
}