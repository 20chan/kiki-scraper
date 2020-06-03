using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kiki.shared {
    public class Comment {
        public bool Deleted { get; set; }
        public string Author { get; set; }
        public DateTime WrittenDate { get; set; }
        public string Content { get; set; }

        public Comment[] SubComments { get; set; }
    }
}