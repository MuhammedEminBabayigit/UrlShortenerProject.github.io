using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UrlShortenerProject.Models
{
    public class ShortenedUrl
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; }
        public string ShortUrl { get; set; }
        //public string Description { get; set; }
        //public string Tag { get; set; }
        //public string SpecificLink { get; set; }
        //public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastDate { get; set; }

        //public int ClickCount { get; set; }

        public bool Status { get; set; }

    }
}
