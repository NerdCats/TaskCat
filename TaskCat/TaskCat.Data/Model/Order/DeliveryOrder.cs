namespace TaskCat.Data.Model.Order
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Linq;
    using Geocoding;
    using Inventory;

    public class DeliveryOrder : OrderModel
    {
        [Required]
        public DefaultAddress From { get; set; }
        [Required]
        public DefaultAddress To { get; set; }

        public string PackageDescription { get; set; }
        public decimal PackageWeight
        {
            get { return PackageList == null ? 0 : PackageList.Sum(x => x.Weight); }
        }

        /// <summary>
        /// Package List to describe what Items would be delivered
        /// </summary>
        /// <remarks>
        /// Price field would be ignored if the order is from a regular User and 
        /// would only be considered if the request is from an Enterprise User
        /// </remarks>
        [Required]
        public List<ItemDetails> PackageList { get; set; }

        /// <summary>
        /// Note to delivery man to provide extra info to delivery man
        /// </summary>
        public string NoteToDeliveryMan { get; set; }

        public DeliveryOrder(string name = null) : base(name, "Delivery")
        {

        }
    }
}
