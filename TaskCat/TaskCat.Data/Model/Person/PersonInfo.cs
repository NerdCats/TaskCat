using TaskCat.Data.Model.Geocoding;

namespace TaskCat.Data.Model.Person
{
    public class PersonInfo
    {
        public string Name { get; set; }
        public DefaultAddress Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
