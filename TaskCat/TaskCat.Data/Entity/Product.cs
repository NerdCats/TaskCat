namespace TaskCat.Data.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a product
    /// </summary>
    public class Product : DbEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Product name is not provided")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Short description for the product not provided")]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the store id
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Store Id not provided")]
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets whether this is a featured product or not
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        /// 
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the order minimum quantity
        /// </summary>
        public int OrderMinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the order maximum quantity
        /// </summary>
        public int OrderMaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this product is returnable 
        /// (a customer is allowed to submit return request with this product)
        /// </summary>
        public bool NotReturnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable buy (Add to cart) button
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is available for Pre-Order
        /// </summary>
        public bool AvailableForPreOrder { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        /// 
        [Range(0.0d, 999999999, ErrorMessage = "Price  must be between 1 and 999999999")]
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        /// 
        [Range(0.0d, 999999999, ErrorMessage = "Old Price  must be between 1 and 999999999")]
        public decimal OldPrice { get; set; }

        /// <summary>
        /// Gets or sets the product special price
        /// </summary>
        /// 
        [Range(0.0d, 999999999, ErrorMessage = "Special Price  must be between 1 and 999999999")]
        public decimal? SpecialPrice { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the special price
        /// </summary>
        /// 
        public DateTime? SpecialPriceStartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the special price
        /// </summary>
        /// 
        public DateTime? SpecialPriceEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this product is marked as new
        /// </summary>
        public bool IsMarkedAsNew { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the new product (set product as "New" from date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }
        /// <summary>
        /// Gets or sets the end date and time of the new product (set product as "New" to date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public decimal Length { get; set; }
        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public decimal Width { get; set; }
        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the available start date and time
        /// </summary>
        public DateTime? AvailableStartDateTimeUtc { get; set; }
        /// <summary>
        /// Gets or sets the available end date and time
        /// </summary>
        public DateTime? AvailableEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets a display order.
        /// This value is used when sorting associated products (used with "grouped" products)
        /// This value is used when sorting home page products
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// Picture Url of the product
        /// </summary>
        public string PicUrl { get; set; }
    }
}
