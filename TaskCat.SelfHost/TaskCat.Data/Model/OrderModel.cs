namespace TaskCat.Data.Model
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Geocoding;
    using JobTasks.Preference;
    using System.Collections.Generic;
    using Order.Delivery;
    using Order;

    [BsonKnownTypes(typeof(DeliveryOrder), typeof(ClassifiedDeliveryOrder))]
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
        /// From where the package should be delivered, this is the pickup location
        /// </summary>
        [Required]
        public DefaultAddress From { get; set; }

        /// <summary>
        // The place the package should be delivered to, this is the delivery location
        /// </summary>
        [Required]
        public DefaultAddress To { get; set; }

        /// <summary>
        /// Order type that defines the type of the order 
        /// and the job that follows
        /// </summary>
        [Required(ErrorMessage = "Order Type not provided")]
        public string Type { get; set; }


        private string _variant = "default";
        /// <summary>
        /// This type would be used for multiple models for single order purpose
        /// All the requests now support json, at some point, we might have to support
        /// Form submits
        /// </summary>
        public string Variant { get { return _variant; } set { _variant = value; } }


        private string _userId;
        /// <summary>
        /// UserId that the order is created for
        /// </summary>
        [Required(ErrorMessage = "User Id Not Provided")]
        public string UserId { get { return _userId; } set { _userId = value; } }

        /// <summary>
        /// VendorId that the order belongs to
        /// </summary>
        public string VendorId { get; set; }

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
        public double? ETAMinutes
        {
            get
            {
                return _ETAMinutes;
            }
            set
            {
                _ETAMinutes = value;
                if (value != null)
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

        /// <summary>
        /// Package Description to describe what the package is all about
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// An invoice id for every order
        /// </summary>
        [Required(ErrorMessage = "Please give an invoice id")]
        public string ReferenceInvoiceId { get; set; }

        public OrderDetails OrderCart { get; set; }


        public OrderModel()
        {

        }

        public OrderModel(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        public List<JobTaskETAPreference> JobTaskETAPreference { get; set; }
    }
}
