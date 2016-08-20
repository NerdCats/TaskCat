namespace TaskCat.Data.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ProductCategory : DbEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Product category name required")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Product category description required")]
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
