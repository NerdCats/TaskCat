namespace TaskCat.Data.Model
{
    using MongoDB.Driver.GeoJsonObjectModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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
