namespace TaskCat.Data.Model.Identity.Registration
{
    using Geocoding;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.ComponentModel.DataAnnotations;

    public class RegistrationModelBase
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address provided")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "A email address must be provided")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Phone number not valid")]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        public string PicUri { get; internal set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public IdentityTypes Type { get; set; }
    }
}
