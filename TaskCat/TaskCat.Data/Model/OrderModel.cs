namespace TaskCat.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class OrderModel
    {
        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_name))
                    return string.Concat("Ride Request for ", _user);
                return _name;
            }

            set { _name = value; }
        }

        [Required(ErrorMessage ="Order Type not provided" , AllowEmptyStrings =false, ErrorMessageResourceName ="Type")]
        public string Type { get; set; }

        /// <summary>
        /// This type would be used for multiple models for single order purpose
        /// All the requests now support json, at some point, we might have to support
        /// Form submits
        /// </summary>
        private string _payloadType = "default";
        public string PayloadType { get { return _payloadType; } set { _payloadType = value; } }

        private string _user = "Anonymous";
        public string User { get { return _user; } set { _user = value; } }

        public OrderModel(string name, string type)
        {
            this.Type = type;
            this.Name = name;
        }
    }
}
