namespace TaskCat.Model.Asset
{
    using Data.Model;
    using Data.Model.Geocoding;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Standard Asset Search Request based on location for a IAssetProvider 
    /// </summary>
    public class AssetSearchRequest
    {
        /// <summary>
        /// Location class to define searches around a place
        /// </summary>
        public DefaultAddress Location { get; set; }
        /// <summary>
        /// Radius in meters to limit search area
        /// </summary>
        public double? Radius { get; set; }
        /// <summary>
        /// Limit results of search, default is 10
        /// </summary>
        public int Limit { get; set; } 
        /// <summary>
        /// Search Strategy, based on search strategy, asset provider should find out
        /// assets for a particular job
        /// </summary>
        /// 
        [JsonConverter(typeof(StringEnumConverter))]
        public SearchStrategy Strategy { get; set; }
    }

    /// <summary>
    /// Search Strategy to define asset search paradigm in a IAssetProvider
    /// </summary>
    public enum SearchStrategy
    {
        /// <summary>
        /// QUICK search paradigm is faster but less intrusive,
        /// Multiple trials and search zone changes are not supported
        /// </summary>
        QUICK,
        /// <summary>
        /// DEEP asset searches would search Asset with preference, multiple zonal
        /// search and what not
        /// </summary>
        DEEP
    }
}