namespace TaskCat.Data.Model.Identity.Response
{
    using System.ComponentModel.DataAnnotations;

    public class GuestUserModel : UserModelBase
    {
        [Required]
        public override string Email
        {
            get
            {
                return base.Email;
            }

            set
            {
                base.Email = value;
            }
        }

        [Required]
        public override IdentityTypes Type
        {
            get
            {
                return IdentityTypes.GUEST_USER;
            }
        }

        [Required]
        public override string UserId
        {
            get
            {
                return "Annonymous";
            }
        }

        [Required]
        public override string PhoneNumber
        {
            get
            {
                return base.PhoneNumber;
            }

            set
            {
                base.PhoneNumber = value;
            }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public GuestUserModel()
        {

        }
    }
}
