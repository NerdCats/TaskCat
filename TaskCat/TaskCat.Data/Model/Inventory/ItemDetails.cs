namespace TaskCat.Data.Model.Inventory
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class ItemDetails : IEquatable<ItemDetails>
    {
        [Required]
        public string Item { get; set; }

        public string PicUrl { get; set; }

        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100000")]
        public int Quantity { get; set; }

        [Range(0, 999999999, ErrorMessage = "Price must be between 0.0 and 999999999")]
        public decimal Price
        {
            get;
            set;
        }

        [Range(0.00, 100, ErrorMessage = "VAT must be a % between 0 and 100")]
        public decimal VAT { get; set; }

        public decimal Total
        {
            get
            {
                return Quantity * Price;
            }
        }

        public decimal VATAmount
        {
            get
            {
                return TotalPlusVAT - Total;
            }
        }

        public decimal TotalPlusVAT
        {
            get
            {
                return Total * (1 + VAT / 100);
            }
        }

        [Range(0.00, 100, ErrorMessage = "Weight must be between 0 and 100")]
        public decimal Weight { get; set; }

        public ItemDetails()
        {
        }

        public bool Equals(ItemDetails other)
        {
            if (!string.Equals(this.Item, other.Item))
                return false;
            if (!string.Equals(this.PicUrl, other.PicUrl))
                return false;
            if (Quantity != other.Quantity)
                return false;
            if (Price != other.Price)
                return false;
            if (VAT != other.VAT)
                return false;
            if (Total != other.Total)
                return false;
            if (VATAmount != other.VATAmount)
                return false;
            if (TotalPlusVAT != other.TotalPlusVAT)
                return false;
            if (Weight != other.Weight)
                return false;

            return true;

        }
    }
}
