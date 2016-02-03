namespace TaskCat.Data.Entity.Identity
{
    using AspNet.Identity.MongoDB;
    using Model.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class User : IdentityUser
    {
        public UserProfile profile { get; set; }
        public User(UserModel model)
        {
            this.UserName = model.UserName;
            this.Email = model.Email;
            this.PhoneNumber = model.PhoneNumber;
            // FIXME: We need to do something about this
            // Emails need to be verified
            if (string.IsNullOrEmpty(Email))
                this.EmailConfirmed = false;
            //FIXME: This we would probably change when we figure out how we are actually authenticating them
            if (string.IsNullOrEmpty(PhoneNumber))
                this.PhoneNumberConfirmed = false;

            //FIXME: This has been done because UserModel is just the same here
            //If we decide to expose different models for different clients things would be a bit different



        }
        
    }
}
