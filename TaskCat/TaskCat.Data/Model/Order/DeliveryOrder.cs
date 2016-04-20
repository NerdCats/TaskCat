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

        [Required]
        public List<ItemDetails> PackageList { get; set; }

        public string NoteToDeliveryMan { get; set; }

        public DeliveryOrder(string name = null) : base(name, "Delivery")
        {

        }
    }
}
