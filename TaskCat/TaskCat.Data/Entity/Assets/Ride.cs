using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskCat.Data.Model.Identity;

namespace TaskCat.Data.Entity.Assets
{
    public class Ride: AssetEntity
    {
        public Ride(UserModel model) : base(model)
        {
            this.FirstName = model.FirstName;
            this.LastName = model.LastName;
            this.Age = model.Age;
            this.Gender = model.Gender;
            this.Address = model.Address;
            this.AssetType = model.UserType;         
        }
    }
}
