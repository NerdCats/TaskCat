namespace TaskCat.Data.Model.Order
{
    using Inventory;
    using MongoDB.Bson.Serialization.Attributes;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class OrderDetails
    {
        /// <summary>
        /// Package List to describe what Items would be delivered
        /// </summary>
        /// <remarks>
        /// Price field would be ignored if the order is from a regular User and 
        /// would only be considered if the request is from an Enterprise User
        /// </remarks>
        /// 
        public List<ItemDetails> PackageList { get; set; }

        /// <summary>
        /// Total Vat Ammount summed up in PackageList
        /// </summary>
        public decimal? TotalVATAmount { get; set; }
        /// <summary>
        /// Sub Total Amount with ItemDetails price
        /// </summary>
        public decimal? SubTotal { get; set; }
        /// <summary>
        /// Service Charge for the delivery, this is the actual part that is
        /// charged for the delivery
        /// </summary>
        public virtual decimal? ServiceCharge { get; set; }
        /// <summary>
        /// Total Weight for the product, can be manuall set or calculated
        /// </summary>
        public virtual decimal? TotalWeight { get; set; }
        /// <summary>
        /// Total to be paid by customer, if the order is generated from
        /// Enterprise user this consists the full amount, if generated 
        /// from end user should only be equal to service charge + payment charge
        /// </summary>
        public virtual decimal? TotalToPay { get; set; }
    }
}
