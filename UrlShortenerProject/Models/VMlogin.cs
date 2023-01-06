using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UrlShortenerProject.Models
{
    public class VMlogin
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Email{ get; set; }
        public string PassWord{ get; set; }
        public bool KeepLoggedIn{ get; set; }
    }
}
