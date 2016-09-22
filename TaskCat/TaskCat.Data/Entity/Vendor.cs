﻿namespace TaskCat.Data.Entity
{
    using Model.Vendor;
    using Model.Vendor.ProfitSharing;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Vendor : DbEntity
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Profit Sharing Strategy not provided")]
        public ProfitSharingStrategy Strategy { get; set; }
        public decimal? FixedDeliveryCharge { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "UserId not provided")]
        public string UserId { get; set; }

        public bool IsAllowedAllOrderTypes
        {
            get
            {
                return AllowedOrderTypes == null
                    || AllowedOrderTypes?.Count == 0;
            }
        }
        public List<VendorOrderPreference> AllowedOrderTypes;
    }
}
