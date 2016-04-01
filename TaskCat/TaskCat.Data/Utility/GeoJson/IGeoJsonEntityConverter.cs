namespace TaskCat.Data.Utility.GeoJson
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using TaskCat.Data.Model.GeoJson;

    public class IGeoJsonEntityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IGeoJsonModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var type = obj["type"];

            if (type == null)
            {
                throw new ArgumentNullException("type", "GeoJson object type is provided null");
            }

            IGeoJsonModel baseGeoJson;
            string geoJsonType = type.Value<string>();
            switch (geoJsonType)
            {
                case "LineString":
                    baseGeoJson = new LineString();
                    break;
                case "Point":
                    baseGeoJson = new Point();
                    break;
                case "Polygon":
                    baseGeoJson = new Polygon();
                    break;
                default:
                    throw new NotSupportedException("GeoJson Entry type invalid/no supported - " + geoJsonType);
            }

            serializer.Populate(obj.CreateReader(), baseGeoJson);
            return baseGeoJson;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
