namespace TaskCat.Data.Model.Identity.Registration
{
    public class AssetRegistrationModel : UserRegistrationModel
    {
        public string NationalId { get; set; }
        public string DrivingLicenceId { get; set; }
    }
}
