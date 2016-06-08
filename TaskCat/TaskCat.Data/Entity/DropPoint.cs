namespace TaskCat.Data.Entity
{
    using Model.Geocoding;
    using System.ComponentModel.DataAnnotations;
    public class DropPoint : DbEntity
    {
        public DefaultAddress Address { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Drop Point Name is not provided")]
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
