namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class Comment : DbEntity
    {
        public string RefId { get; set; }
        public string EntityType { get; set; }
        public string CommentText { get; set; }
        public string RefCommentId { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
    }   
}
