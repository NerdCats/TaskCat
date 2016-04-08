namespace TaskCat.Data.Entity.ShadowCat
{
    using Model.GeoJson;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Asset Location Entity holds    
    /// location for a specific asset
    /// </summary>
    public class AssetLocation : DbEntity
    {
        [JsonProperty(PropertyName = "asset_id")]
        public string Asset_Id { get; set; }
        [JsonProperty(PropertyName = "point")]
        public Point Point { get; set; }
        [JsonProperty(PropertyName = "device")]
        public AssetDevice Device { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonIgnore]
        public bool IgnoreAssetId = false;

        public bool ShouldSerializeAsset_Id()
        {
            return IgnoreAssetId;
        }

        public AssetLocation()
        {

        }

        public AssetLocation(bool ignoreAssetId)
        {
            IgnoreAssetId = ignoreAssetId;
        }
    }
}
