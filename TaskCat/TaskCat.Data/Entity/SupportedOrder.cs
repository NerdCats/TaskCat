using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCat.Data.Entity
{
    public class SupportedOrder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [Required(ErrorMessage ="You must provide an action name like GO, Fetch, GET etc")]
        public string ActionName { get; set; }

        [Required(ErrorMessage = "You must provide a Name of the order that will be displayed in the app like Ride, Food, Courier")]
        public string OrderName { get; set; }
        public string ImageUrl { get; set; }

        [Required(ErrorMessage ="You must provide the the supported order code, currently supported orders will be found in this URL: /api/supportedorder/get")]
        public SupportedOrder OrderCode { get; set; }        
    }

    enum SupportedOrderCode
    {
        FOOD,
        RIDE,
        FETCH,
        PLUMBER
    }
}
