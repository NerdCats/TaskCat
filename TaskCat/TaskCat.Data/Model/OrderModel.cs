﻿namespace TaskCat.Data.Model
{
    using GeoJson;
    using Microsoft.AspNet.Identity;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Principal;

    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class OrderModel
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set { _name = value; }
        }

        [Required(ErrorMessage ="Order Type not provided" )]
        public string Type { get; set; }

        /// <summary>
        /// This type would be used for multiple models for single order purpose
        /// All the requests now support json, at some point, we might have to support
        /// Form submits
        /// </summary>
        private string _payloadType = "default";
        public string PayloadType { get { return _payloadType; } set { _payloadType = value; } }

        private string _userId;
        public string UserId { get { return _userId; } set { _userId = value; } }

        /// <summary>
        /// Location where the order was originated from
        /// </summary>
        public Point OrderLocation { get; set; }

        private DateTime? _eta;
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
