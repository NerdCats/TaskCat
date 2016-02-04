using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCat.Data.Model.Identity
{
    public class AssetModel : UserModel
    {
        public string NationalId { get; set; }
        public string DrivingLicenceId { get; set; }
    }
}
