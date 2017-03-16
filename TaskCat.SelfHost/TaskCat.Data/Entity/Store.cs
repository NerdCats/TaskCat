namespace TaskCat.Data.Entity
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a store
    /// </summary>
    public class DataTag : DbEntity
    {
        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Store name is not provided")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the store URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the enterprise user id associated with it
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enterprise User Id not provided")]
        public string EnterpriseUserId { get; set; }

        /// <summary>
        /// ProductCategories
        /// </summary>
        public List<ProductCategory> ProductCategories { get; set; }

        /// <summary>
        /// Store cover pic url for store
        /// </summary>
        public string CoverPicUrl { get; set; }
    }
}
