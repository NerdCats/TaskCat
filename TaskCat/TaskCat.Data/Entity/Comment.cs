namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class Comment : DbEntity
    {
        public string RefId { get; set; }
        public string EntityType { get; set; }
        public string CommentText { get; set; }
        public string RefCommentId { get; set; }
    }   
}
