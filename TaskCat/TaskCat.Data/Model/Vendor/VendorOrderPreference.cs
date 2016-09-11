namespace TaskCat.Data.Model.Vendor
{
    public class VendorOrderPreference
    {
        public string OrderType { get; set; }
        public string PayloadType { get; set; } = "default";
    }
}
