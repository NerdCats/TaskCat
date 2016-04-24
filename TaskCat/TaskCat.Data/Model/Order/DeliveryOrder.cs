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

        #region Invoice
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
        /// Total Vat Ammount summed up in PackageList
        /// </summary>
        public decimal TotalVATAmount { get; set; }
        /// <summary>
        /// Sub Total Amount with ItemDetails price
        /// </summary>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Service Charge for the delivery, this is the actual part that is
        /// charged for the delivery
        /// </summary>
        public virtual decimal ServiceCharge { get; set; }
        /// <summary>
        /// Total Weight for the product, can be manuall set or calculated
        /// </summary>
        public virtual decimal TotalWeight{ get; set; }
        /// <summary>
        /// Total to be paid by customer, if the order is generated from
        /// Enterprise user this consists the full amount, if generated 
        /// from end user should only be equal to service charge + payment charge
        /// </summary>
        public virtual decimal TotalToPay { get; set; }

        #endregion

        /// <summary>
        /// Note to delivery man to provide extra info to delivery man
        /// </summary>
        public string NoteToDeliveryMan { get; set; }

        public DeliveryOrder(string name = null) : base(name, "Delivery")
        {

        }
    }
}
