namespace TaskCat.Data.Model.GeoJson
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Utility.GeoJson;
    public class Polygon : IGeoJsonEntity
    {
        public string type { get; set; }

        List<List<List<double>>> _coordinates;
        public List<List<List<double>>> coordinates
        {
            get { return _coordinates; }
            set
            {
                //FIXME: Should have a utility that actually helps determine these
                foreach (var polygonPart in value)
                {
                    if (polygonPart.Count < 4)
                        throw new InvalidOperationException("coordinates doesn't have enough coordinates to create a LinearRing");
                    if (!((polygonPart.Last().First() == polygonPart.First().First())
                        && (polygonPart.Last().Last() == polygonPart.First().Last())))
                        throw new InvalidOperationException("coordinates doesn't resemble a LinearRing set of coordinates");

                    var isValid = polygonPart.All(x => LocationUtility.IsValidLocation(x.Last(), x.First()));
                    if (!isValid)
                        throw new ArgumentException("coordinates contains invalid location");
                }

                _coordinates = value;
            }
        }

        public Polygon()
        {
            this.type = "Polygon";
        }

        public Polygon(List<List<List<double>>> coords)
        {
            this.coordinates = coords;
        }
    }
}
