namespace TaskCat.Data.Model.Identity.Response
{
    using Entity.Identity;
    using TaskCat.Data.Model.Identity.Profile;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Entity;

    public class UserModel
    {
    
        public bool IsUserAuthenticated = false;
        // FIXME: Do I really need the id here? because I can find anyone by username in
        // public profile, and when Im logged in, I already know my profile
        [JsonProperty(PropertyName = "Id")]
        public string UserId { get; set; }
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
            this.UserId = user.Id;
            this.Type = user.Type;
            this.PhoneNumber = user.PhoneNumberConfirmed ? user.PhoneNumber : string.Empty;
            this.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            this.Email = user.Email;
            this.EmailConfirmed = user.EmailConfirmed;
            this.Profile = user.GetType() == typeof(Asset) ? user.Profile as AssetProfile : user.Profile;

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
