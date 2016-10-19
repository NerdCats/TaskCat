namespace TaskCat.Data.Model
{
    public class VehicleInfo
    {
        public string DeviceID { get; set; }

        public string RegistrationNumber { get; set; }
        public string ChasisNumber { get; set; }
        public int? Wheels { get; set; }

        public string Vendor { get; set; }
        public string Model { get; set; }

        public string Class { get; set; }
    }
}
