namespace TaskCat.Data.Utility.GeoJson
{
    using Model.GeoJson;
    using System;
    using System.Linq;

    public class LocationUtility
    {
        public static bool IsValidLocation(double lat, double lon)
        {
            return (lat >= -90 && lat <= 90) && (lon >= -180 && lon <= 180);
        }

        public static bool IsValidLocation(Point location)
        {
            if (location.coordinates == null)
                throw new ArgumentNullException("Location coordinates are null");
            if (location.coordinates.Count > 0)
                throw new ArgumentException("coordinates should have only latitude and longitude");

            return IsValidLocation(location.coordinates.Last(), location.coordinates.First());
        }

    }
}
