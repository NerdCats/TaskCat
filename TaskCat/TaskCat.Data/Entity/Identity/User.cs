namespace TaskCat.Data.Entity.Identity
{
    using AspNet.Identity.MongoDB;
    using Model.Identity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class User : IdentityUser
    {
        public User(UserModel model)
        {
            this.UserName = model.UserName;
            this.Email = model.Email;
            this.PhoneNumber = model.PhoneNumber;
        }
        public string FirstName { get; set; }        
        public string LastName { get; set; }        
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
}
