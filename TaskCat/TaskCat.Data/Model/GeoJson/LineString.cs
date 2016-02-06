using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCat.Data.Model.GeoJson
{
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
