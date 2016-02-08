namespace TaskCat.Data.Model.Identity.Response
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Entity.Identity;
    using TaskCat.Data.Model.Identity.Profile;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class UserModel
    {
        [JsonIgnore]
        public bool IsUserAuthenticated = false;
        // FIXME: Do I really need the id here? because I can find anyone by username in
        // public profile, and when Im logged in, I already know my profile
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public IdentityTypes Type { get; set; }     
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public UserProfile Profile { get; set; }

        public UserModel()
        {

        }

        public UserModel(User user, bool isUserAuthenticated = false)
        {
            this.Id = user.Id;
            this.Type = user.Type;
            this.PhoneNumber = user.PhoneNumberConfirmed ? user.PhoneNumber : string.Empty;
            this.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            this.Email = user.Email;
            this.EmailConfirmed = user.EmailConfirmed;
            this.Profile = user.Profile;

            this.IsUserAuthenticated = isUserAuthenticated;
        }

        public bool ShouldSerializePhoneNumberConfirmed()
        {
            return IsUserAuthenticated;
        }

        public bool ShouldSerializeId()
        {
            return IsUserAuthenticated;
        }

        public bool ShouldSerializeEmailConfirmed()
        {
            return IsUserAuthenticated;
        }

    }
}
