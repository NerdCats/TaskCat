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
        // FIXME: this guy would need a JsonConverter when you'd deserialize him
        public UserProfile Profile { get; set; }

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
            switch (model.AssetType)
            {
                case AssetTypes.FETCHER:
                    this.Profile = new UserProfile(model);
                    break;
                default:
                    if (model.GetType() != typeof(AssetModel))
                        throw new InvalidOperationException("AssetProfile type should be used to populate Users who are basically Assets thus need an AssetProfile");

                    this.Profile = new AssetProfile(model as AssetModel);
                    break;
            }

        }
        
    }
}
