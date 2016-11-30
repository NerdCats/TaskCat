namespace TaskCat.Data.Model.Identity.Response
{
    using Entity.Identity;
    using Profile;

    public class UserModel : UserModelBase
    {  
        public string UserName { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public IdentityProfile Profile { get; set; }

        public UserModel()
        {

        }

        public UserModel(User user, bool isUserAuthenticated = false)
        {
            this.UserId = user.Id;
            this.Type = user.Type;
            this.PhoneNumber = user.PhoneNumber;
            this.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            this.Email = user.Email;
            this.EmailConfirmed = user.EmailConfirmed;
            this.Profile = user.GetType() == typeof(User) ? user.Profile : null;
            this.UserName = user.UserName;

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

        public override string ToString()
        {
            string name = this.Profile?.GetName();
            return string.IsNullOrEmpty(name)
                ? this.UserName : name;
        }
    }
}
