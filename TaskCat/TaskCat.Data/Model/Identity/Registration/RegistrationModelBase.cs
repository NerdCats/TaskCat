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

        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "A valid email address must be provided")]
        public string Email { get; set; }

        [RegularExpression(@"(\+8801\d{9})|(01\d{9})", ErrorMessage = @"Please provide a valid Bangladeshi Phone Number, ex(+)")]
        [Display(Name = "PhoneNumber")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A valid phone number must be provided")]
        public string PhoneNumber { get; set; }

        public string PicUri { get; internal set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public IdentityTypes Type { get; set; }

        public DefaultAddress Address { get; set; }
    }
}
