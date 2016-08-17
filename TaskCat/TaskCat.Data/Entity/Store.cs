namespace TaskCat.Data.Entity
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a store
    /// </summary>
    public class Store : DbEntity
    {
        /// <summary>
        /// Gets or sets the store name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Web url of the Store
        /// </summary>
        public string WebUrl { get; set; }

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
