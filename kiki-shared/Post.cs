using System;
using MongoDB.Bson.Serialization.Attributes;

namespace kiki.shared {
    public class Post {
        [BsonId]
        public int Id { get; set; }
        public int Community { get; set; }
        public DateTime ScrappedDate { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime WrittenDate { get; set; }
        public string Content { get; set; }

        public Comment[] Comments { get; set; }
    }
}