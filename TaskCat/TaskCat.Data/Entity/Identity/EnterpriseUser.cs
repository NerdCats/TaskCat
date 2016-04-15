namespace TaskCat.Data.Entity.Identity
{
    using System.Collections.Generic;
    using TaskCat.Data.Model.Identity.Profile;
    using TaskCat.Data.Model.Identity.Registration;

    public class EnterpriseUser : User
    {
        public EnterpriseUser(EnterpriseUserRegistrationModel model, EnterpriseUserProfile profile) 
            : base(model, profile)
        {
            this.Roles = new List<string>();
            Roles.Add(RoleNames.ROLE_ENTERPRISE);
        }
    }
}
