using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCat.Data.Model
{
    public class AssetProfile
    {
        [Required(ErrorMessage ="First Name is not provided")]
        public string FirstName { get; set; }

        [Required(ErrorMessage ="Last Name is not provided")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="User name is not Unique or not provided")]
        public string UserName { get; set; }

        public string Email { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
