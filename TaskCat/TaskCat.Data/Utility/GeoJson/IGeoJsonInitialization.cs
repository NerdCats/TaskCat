namespace TaskCat.Data.Utility.GeoJson
{
    using MongoDB.Bson.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Model.GeoJson;

    public class IGeoJsonInitialization
    {
        public static void Initiate()
        {
            BsonClassMap.RegisterClassMap<Polygon>();
            BsonClassMap.RegisterClassMap<LineString>();
            BsonClassMap.RegisterClassMap<Point>();
        }
    }
}
