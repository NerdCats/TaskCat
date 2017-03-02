namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class StockItem: DbEntity
    {
        /// <summary>
        /// Reference id for the specific entity type
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "RefId not provided")]
        public string RefId { get; set; }

        /// <summary>
        /// Entity type associated with the reference id
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "EntityType not provided")]
        public string RefEntityType { get; set; }

        /// <summary>
        /// Name of the item to be stored
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Item name not provided")]
        public string Item { get; set; }

        /// <summary>
        /// Picture url of the item, if there is any
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// Quantity of the item to be stored
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Creation time of the StockItem entry 
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Last modified time of the StockItem entry 
        /// </summary>
        public DateTime? ModifiedTime { get; set; }
    }
}
