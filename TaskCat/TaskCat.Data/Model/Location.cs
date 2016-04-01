namespace TaskCat.Data.Model
{
    using TaskCat.Data.Model.GeoJson;

    public class Location
    {
        public Point Point { get; set; }
        public string Address { get; set; }

        public Location()
        {

        }
    }
}
