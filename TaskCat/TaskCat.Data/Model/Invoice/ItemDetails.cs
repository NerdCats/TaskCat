namespace TaskCat.Data.Model.Invoice
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ItemDetails
    {
        [Required]
        public string Item { get; set; }

        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100000")]
        public int Quantity { get; set; }

        [Range(0.1, 999999999, ErrorMessage = "Price must be between 0.01 and 999999999")]
        public decimal Price
        {
            get;
            set;
        }

        [Range(0.00, 100, ErrorMessage = "VAT must be a % between 0 and 100")]
        public decimal VAT { get; set; }


        public DateTime? CreatedTime { get; set; } = DateTime.UtcNow;

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

        [Range(0.00, 100, ErrorMessage = "VAT must be a % between 0 and 100")]
        public decimal Weight { get; set; }

        public ItemDetails()
        {
        }
    }
}
