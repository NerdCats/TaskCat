namespace TaskCat.Data.Model.Identity.Response
{
    using Entity.Identity;
    using Profile;

    public class EnterpriseUserModel : UserModel
    {
        public EnterpriseUserModel()
        {

        }

        public EnterpriseUserModel(EnterpriseUser enterpriseUser, bool isUserAuthenticated = false) 
            : base(enterpriseUser, isUserAuthenticated)
        {
            this.Profile = enterpriseUser.Profile as EnterpriseUserProfile;
        }
    }
}
