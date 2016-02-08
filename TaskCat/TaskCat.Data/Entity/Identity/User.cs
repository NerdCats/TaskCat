namespace TaskCat.Data.Entity.Identity
{
    using AspNet.Identity.MongoDB;
    using Model.Identity;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Model.Identity.Profile;
    using Model.Identity.Registration;

    [BsonDiscriminator(Required = true)]
    [BsonKnownTypes(typeof(Asset))]
    public class User : IdentityUser
    {
        // FIXME: this guy would need a JsonConverter when you'd deserialize him
        public UserProfile Profile { get; set; }
        public IdentityTypes Type { get; set; }

        public User(UserRegistrationModel model, UserProfile profile)
        {
            this.UserName = model.UserName;
            this.Email = model.Email;
            this.PhoneNumber = model.PhoneNumber;
            this.Type = model.Type;

            this.Roles = new List<string>();
            Roles.Add(RoleNames.ROLE_USER);

            // FIXME: We need to do something about this
            // Emails need to be verified
            if (string.IsNullOrEmpty(Email))
                this.EmailConfirmed = false;
            //FIXME: This we would probably change when we figure out how we are actually authenticating them
            if (string.IsNullOrEmpty(PhoneNumber))
                this.PhoneNumberConfirmed = false;

            //FIXME: This has been done because UserModel is just the same here
            //If we decide to expose different models for different clients things would be a bit different
            this.Profile = profile;
        }

        protected User(UserRegistrationModel model, UserProfile profile, string role) : this(model, profile)
        {
            this.Roles = this.Roles ?? new List<string>();
            Roles.Add(role);
        }
        
    }
}
