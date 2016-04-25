namespace TaskCat.Data.Model
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Geocoding;
    using Payment;

    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class OrderModel
    {
        private string _name;
        /// <summary>
        /// Name for the order
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set { _name = value; }
        }

        /// <summary>
        /// Order type that defines the type of the order 
        /// and the job that follows
        /// </summary>
        [Required(ErrorMessage ="Order Type not provided" )]
        public string Type { get; set; }

        
        private string _payloadType = "default";
        /// <summary>
        /// This type would be used for multiple models for single order purpose
        /// All the requests now support json, at some point, we might have to support
        /// Form submits
        /// </summary>
        public string PayloadType { get { return _payloadType; } set { _payloadType = value; } }

        
        private string _userId;
        /// <summary>
        /// UserId that the order is created for
        /// </summary>
        [Required(ErrorMessage = "User Id Not Provided")]
        public string UserId { get { return _userId; } set { _userId = value; } }

        /// <summary>
        /// Location where the order was originated from
        /// </summary>
        public DefaultAddress OrderLocation { get; set; }

        private DateTime? _eta;
        /// <summary>
        /// ETA suggested for Order
        /// </summary>
        public DateTime? ETA
        {
            get { return _eta; }
            set
            {
                if (_eta != null)
                    _eta = value;
            }
        }

        private double? _ETAMinutes;
        /// <summary>
        /// ETA to be described as minutes 
        /// </summary>
        public double? ETAMinutes {
            get {
                return _ETAMinutes;
            }
            set {
                _ETAMinutes = value;
                if(value!=null)
                    ETA = DateTime.Now.Add(TimeSpan.FromMinutes(value.Value));
            }
        }

        /// <summary>
        /// A supported payment method id
        /// </summary>
        /// <remarks>
        /// A supported payment method list can be accessed GET /api/payment
        /// </remarks>
        /// 
        [Required]
        public string PaymentMethod { get; set; }
        
        public OrderModel()
        {

        }

        public OrderModel(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
