namespace TaskCat.Data.Model.GeoJson
{
    using System.Collections.Generic;

    public class Point : IGeoJsonModel
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }

        public Point()
        {
            this.type = "Point";
        }

        public Point(double lon, double lat) : this()
        {
            this.coordinates = new List<double>();
            coordinates.Add(lon);
            coordinates.Add(lat);
        }

        public Point(List<double> coords)
        {
            this.coordinates = coords;
        }
    }
}
