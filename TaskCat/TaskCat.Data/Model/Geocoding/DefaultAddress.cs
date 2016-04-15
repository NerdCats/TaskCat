namespace TaskCat.Data.Model.Geocoding
{
    using GeoJson;
    using System;
    using System.Text;
    public class DefaultAddress : AddressBase
    {
        public override string Address
        {
            get
            {
                return string.IsNullOrEmpty(base.Address) ? GenerateAddress() : base.Address;
            }

            set
            {
                base.Address = value;
            }
        }

        public DefaultAddress(string formattedAddress, Point point) : base(formattedAddress, "Default", point)
        {
            
        }

        public DefaultAddress(string addressLine1, string addressLine2, string country, string city, Point point) : base(string.Empty, "Default", point)
        {
            if (string.IsNullOrWhiteSpace(addressLine1))
                throw new ArgumentNullException("address line 1 is blank or empty");
            
            this.AddressLine1 = addressLine1;
            this.AddressLine2 = addressLine2;
            this.Country = country;
            this.City = city;
        }

        public string PostalCode { get; set; }
        public string Floor { get; set; }
        public string HouseNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        private string GenerateAddress()
        {
            StringBuilder sb = new StringBuilder();
            if(string.IsNullOrWhiteSpace(AddressLine1))
                throw new ArgumentNullException("address line 1 is blank or empty");

            if (!string.IsNullOrWhiteSpace(HouseNumber)) sb.AppendFormat("House = {0}", HouseNumber);
            if (!string.IsNullOrWhiteSpace(Floor)) sb.AppendFormat(", {0} Floor, ", Floor);

            sb.AppendFormat("{0}", AddressLine1);

            if (!string.IsNullOrWhiteSpace(City)) sb.AppendFormat(", {0}", City);
            if (!string.IsNullOrWhiteSpace(City) && string.IsNullOrWhiteSpace(PostalCode)) sb.AppendFormat("-{0}", PostalCode);
            if (!string.IsNullOrWhiteSpace(State)) sb.AppendFormat(", {0}", State);
            if (!string.IsNullOrWhiteSpace(Country)) sb.AppendFormat(", {0}", Country);

            return sb.ToString();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
