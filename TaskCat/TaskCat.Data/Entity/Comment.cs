namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class Comment : DbEntity
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "RefId not provided")]
        public string RefId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "EntityType not provided")]
        public string EntityType { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "CommentText not provided")]
        public string CommentText { get; set; }

        public string RefCommentId { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? LastModified { get; set; }
    }   
}
