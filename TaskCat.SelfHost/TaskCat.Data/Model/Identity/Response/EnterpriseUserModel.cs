namespace TaskCat.Data.Model.Identity.Response
{
    using System;
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
            this.VendorId = enterpriseUser.VendorId;
            this.VendorSubscriptionDate = enterpriseUser.VendorSubscriptionDate;
        }

        public string VendorId { get; private set; }
        public DateTime? VendorSubscriptionDate { get; private set; }
    }
}
