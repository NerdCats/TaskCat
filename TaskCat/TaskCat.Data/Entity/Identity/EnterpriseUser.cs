﻿namespace TaskCat.Data.Entity.Identity
{
    using System;
    using System.Collections.Generic;
    using Model.Identity.Profile;
    using Model.Identity.Registration;

    public class EnterpriseUser : User
    {
        public EnterpriseUser(EnterpriseUserRegistrationModel model, EnterpriseUserProfile profile) 
            : base(model, profile)
        {
            this.Roles = new List<string>();
            Roles.Add(RoleNames.ROLE_ENTERPRISE);
        }

        public string VendorProfileId { get; set; }
        public DateTime? VendorSubscriptionDate { get; set; }
    }
}
