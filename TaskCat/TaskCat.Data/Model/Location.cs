using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCat.Data.Model
{
    public class Location
    {
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Point { get; set; }
        public string Address { get; set; }
    }
}
