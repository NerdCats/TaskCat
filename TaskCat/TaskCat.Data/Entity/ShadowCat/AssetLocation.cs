namespace TaskCat.Data.Entity.ShadowCat
{
    using Model.GeoJson;
    using System;

    /// <summary>
    /// Asset Location Entity holds    
    /// location for a specific asset
    /// </summary>
    public class AssetLocation
    {
        public string asset_id { get; set; }
        public Point point { get; set; }
        public AssetDevice device { get; set; }
        public DateTime timestamp { get; set; }
    }
}
