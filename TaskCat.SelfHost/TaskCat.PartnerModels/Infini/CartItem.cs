using System.Collections.Generic;

namespace TaskCat.PartnerModels.Infini
{
    public class CartItem
    {
        public string rowid { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string qty { get; set; }
        public string price { get; set; }
        public int subtotal { get; set; }
    }
}
