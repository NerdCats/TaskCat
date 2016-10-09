﻿namespace TaskCat.Data.Model.Inventory
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;

    [BsonIgnoreExtraElements(Inherited = true)]
    public class ItemDetails
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
    }
}
