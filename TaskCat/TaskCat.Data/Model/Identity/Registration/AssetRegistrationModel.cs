namespace TaskCat.Data.Model.Identity.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AssetRegistrationModel : UserRegistrationModel
    {
        public string NationalId { get; set; }
        public string DrivingLicenceId { get; set; }
    }
}
