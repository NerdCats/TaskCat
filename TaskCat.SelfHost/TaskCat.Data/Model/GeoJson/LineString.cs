namespace TaskCat.Data.Model.GeoJson
{
    using System.Collections.Generic;

    public class LineString : IGeoJsonModel
    {
        public string type { get; set; }
        public List<List<double>> coordinates { get; set; }

        public LineString()
        {
            type = "LineString";
        }
        public LineString(List<List<double>> coords)
        {
            coordinates = coords;
        }
    }
}
