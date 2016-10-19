namespace TaskCat.Data.Model.Person
{
    using TaskCat.Data.Model.Geocoding;
    public class PersonInfo
    {
        public string UserRef { get; set; }
        public string Name { get; set; }
        public DefaultAddress Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
