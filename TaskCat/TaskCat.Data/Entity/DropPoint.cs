namespace TaskCat.Data.Entity
{
    using Model.Geocoding;

    public class DropPoint : DbEntity
    {
        public DefaultAddress Address { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        public DropPoint()
        {

        }

        public DropPoint(string userId, string name, DefaultAddress address)
        {
            this.Name = name;
            this.UserId = userId;
            this.Address = address;
        }
    }
}
