namespace TaskCat.Data.Model.Identity.Profile
{
    using System.ComponentModel.DataAnnotations;

    public class ContactUpdateModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [RegularExpression(@"(\+8801\d{9})|(01\d{9})", ErrorMessage = @"Please provide a valid Bangladeshi Phone Number, ex(+)")]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
