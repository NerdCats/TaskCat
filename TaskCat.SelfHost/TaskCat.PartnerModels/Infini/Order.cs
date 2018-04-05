using System.Collections.Generic;

namespace TaskCat.PartnerModels.Infini
{
    public class Order
    {
        public int id { get; set; }
        public string shipping_id { get; set; }
        public string product { get; set; }
        public int qty { get; set; }
        public int unit_price { get; set; }
        public int total { get; set; }
        public int order_status { get; set; }
        public int user_id { get; set; }
        public int vendor_id { get; set; }
        public UserAddress user_address { get; set; }
        public List<VendorAddress> vendor_address { get; set; }
    }
}
