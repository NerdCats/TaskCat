using TaskCat.Data.Model.Identity.Registration;

namespace TaskCat.Data.Model.Identity.Profile
{
    public class EnterpriseUserProfile : IdentityProfile
    {
        public string ContactPersonName { get; set; }
        public string Website { get; set; }

        public EnterpriseUserProfile()
        {

        }

        public EnterpriseUserProfile(EnterpriseUserRegistrationModel enterpriseUserModel)
        {
            this.ContactPersonName = enterpriseUserModel.ContactPersonName;
            this.Website = enterpriseUserModel.Website;
        }
    }
}
