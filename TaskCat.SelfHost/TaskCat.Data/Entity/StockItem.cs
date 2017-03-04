namespace TaskCat.Data.Entity
{
    using Model;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class StockItem : DbEntity
    {
        /// <summary>
        /// Reference id for the specific entity type, usually ignore if RefEntityType is not set
        /// </summary>
        public string RefId { get; set; }

        /// <summary>
        /// Entity type associated with the reference id, usually ignored RefId id is not set
        /// </summary>
        public string RefEntityType { get; set; }

        /// <summary>
        /// Name of the item to be stored
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// Picture url of the item, if there is any
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        /// Quantity of the item to be stored
        /// </summary>
        /// 
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string Location { get; set; }

        /// <summary>
        /// Creation time of the StockItem entry 
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Last modified time of the StockItem entry 
        /// </summary>
        public DateTime? ModifiedTime { get; set; }

        public static StockItem FromModel(StockItemModel model)
        {
            var isReferenced = !string.IsNullOrWhiteSpace(model.RefId)
                && !string.IsNullOrWhiteSpace(model.RefEntityType);

            if (!isReferenced)
            {
                model.RefId = null;
                model.RefEntityType = null;
            }

            return new StockItem()
            {
                CreateTime = DateTime.UtcNow,
                ModifiedTime = DateTime.UtcNow,
                Item = model.Item,
                PicUrl = model.PicUrl,
                Quantity = model.Quantity,
                RefEntityType = model.RefEntityType,
                RefId = model.RefId,
                Location = model.Location
            };
        }
    }
}
