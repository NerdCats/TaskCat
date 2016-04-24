namespace TaskCat.Data.Model.Order
{
    using System.ComponentModel.DataAnnotations;
    using Geocoding;

    public class DeliveryOrder : OrderModel
    {
        /// <summary>
        /// From where the package should be delivered, this is the pickup location
        /// </summary>
        [Required]
        public DefaultAddress From { get; set; }
        /// <summary>
        // The place the package should be delivered to, this is the delivery location
        /// </summary>
        [Required]
        public DefaultAddress To { get; set; }

        /// <summary>
        /// Package Description to describe what the package is all about
        /// </summary>
        public string PackageDescription { get; set; }
        
        [Required]
        public OrderDetails OrderCart { get; set; }

        /// <summary>
        /// Note to delivery man to provide extra info to delivery man
        /// </summary>
        public string NoteToDeliveryMan { get; set; }

        public DeliveryOrder(string name = null) : base(name, "Delivery")
        {

        }
    }
}
