namespace TaskCat.Data.Model.Geocoding
{
    using GeoJson;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    public class DefaultAddress : AddressBase
    {
        public override string Address
        {
            get
            {
                return GenerateAddress();
            }
        }

        [JsonConstructor]
        public DefaultAddress() 
        {
            Provider = "Default";
        }

        public DefaultAddress(string formattedAddress, Point point) : base(formattedAddress, "Default", point)
        {
            
        }

        public DefaultAddress(string addressLine1, string addressLine2,  string city, string postcode, string country, Point point) : this(addressLine1, point)
        {
            if (string.IsNullOrWhiteSpace(addressLine1))
                throw new ArgumentNullException("address line 1 is blank or empty");
            
            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.Country = country;
            this.City = city;
            this.PostalCode = postcode;
            
        }

        public string PostalCode { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Floor is required")]
        public string Floor { get; set; }
        public string HouseNumber { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage = "AddressLine 1 is required")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Country { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required")]
        public string City { get; set; }
        public string State { get; set; }

        public string GenerateAddress()
        {
            if (AddressLine1 != null)
            {
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(HouseNumber)) sb.AppendFormat("House = {0}", HouseNumber);
                if (!string.IsNullOrWhiteSpace(Floor)) sb.AppendFormat(", {0} Floor, ", Floor);

                sb.AppendFormat("{0}", AddressLine1);

                if (!string.IsNullOrWhiteSpace(AddressLine2)) sb.AppendFormat(", {0}", AddressLine2);
                if (!string.IsNullOrWhiteSpace(City)) sb.AppendFormat(", {0}", City);
                if (!string.IsNullOrWhiteSpace(City) && !string.IsNullOrWhiteSpace(PostalCode)) sb.AppendFormat("-{0}", PostalCode);
                if (!string.IsNullOrWhiteSpace(State)) sb.AppendFormat(", {0}", State);
                if (!string.IsNullOrWhiteSpace(Country)) sb.AppendFormat(", {0}", Country);

                return sb.ToString();
            }
            else
            {
                return base.Address;
            }
        }

        public override string ToString()
        {
            return GenerateAddress();
        }
    }
}
