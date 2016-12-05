using System;
using MongoDB.Bson.Serialization.Attributes;
using TaskCat.Data.Model.Identity.Registration;

namespace TaskCat.Data.Model.Identity.Profile
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public class EnterpriseUserProfile : IdentityProfile
    {
        public string CompanyName { get; set; }
        public string Website { get; set; }

        public EnterpriseUserProfile()
        {

        }

        public EnterpriseUserProfile(EnterpriseUserRegistrationModel enterpriseUserModel)
        {
            this.CompanyName = enterpriseUserModel.ContactPersonName;
            this.Website = enterpriseUserModel.Website;
        }

        public override string GetName()
        {
            return CompanyName;
        }
    }
}
