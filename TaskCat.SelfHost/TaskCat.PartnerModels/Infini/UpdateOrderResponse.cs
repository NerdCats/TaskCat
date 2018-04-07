using System.Collections.Generic;

namespace TaskCat.PartnerModels.Infini
{
    public class UpdateOrderResponse
    {
        public string status { get; set; }
        public List<Order> order { get; set; }
    }
}
