namespace TaskCat.Data.Model.Identity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UserModel
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

        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email")]
        [Required(ErrorMessage ="A valid email address must be provided")]        
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name ="Confirm Email address")]
        [Compare("Email", ErrorMessage = "The email and confirmation email do not match.")]
        public string EmailConfirmed { get; set; }

    }
}
