namespace TaskCat.Data.Entity
{
    using Identity;
    using Model.Identity.Profile;
    using Model.Identity.Registration;

    public class EnterpriseUser : User
    {
        public EnterpriseUser(
            UserRegistrationModel model,
            UserProfile profile) : base(model, profile, RoleNames.ROLE_ENTERPRISE)
        {

        }
    }
}
