namespace TaskCat.Data.Model.Geocoding
{
    using GeoJson;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Most basic and generic form of address.
    /// Just the full address string and a lat/long
    /// </summary>
    public abstract class AddressBase
    {
        string formattedAddress = string.Empty;
        string provider = string.Empty;

        [JsonConstructor]
        protected AddressBase()
        {

        }
        public AddressBase(string formattedAddress, string provider, Point point)
        {
            Address = formattedAddress;
            Provider = provider;
            Point = point;
        }

        public virtual Point Point { get; set; }

        public virtual string Address
        {
            get { return formattedAddress; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("FormattedAddress is null or blank");

                formattedAddress = value.Trim();
            }
        }

        public virtual string Provider
        {
            get { return provider; }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Provider can not be null or blank");

                provider = value;
            }
        }

        public override string ToString()
        {
            return this.Address;
        }
    }
}
