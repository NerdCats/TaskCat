namespace TaskCat.Data.Entity
{
    using System;
    public class ProductCategory : DbEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Utc representation of the time this was created
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Last UTC timestamp this category was modified
        /// </summary>
        public DateTime? LastModified { get; set; }
    }
}
