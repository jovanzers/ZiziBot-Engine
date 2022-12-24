using MongoDB.Bson;

namespace ZiziBot.DataSource.MongoDb.Entities;

public class EntityBase
{
    public ObjectId Id { get; set; }

    public int Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdateTime { get; set; }
}