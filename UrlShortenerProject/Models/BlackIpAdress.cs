using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UrlShortenerProject.Models
{
    public class BlackIpAdress
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserIP { get; set; }
    }
}
