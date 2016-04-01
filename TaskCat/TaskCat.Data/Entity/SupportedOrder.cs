using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage ="You must provide the the supported order code. supportedorder/get")]        
        public string OrderCode { get; set; }        
    }       
}
