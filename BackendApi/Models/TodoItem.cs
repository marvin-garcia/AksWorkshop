using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendApi.Models
{
    public class TodoItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("IsComplete")]
        public bool IsComplete { get; set; }
    }
}
