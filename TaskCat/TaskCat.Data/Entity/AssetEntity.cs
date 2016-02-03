using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskCat.Data.Entity.Identity;
using TaskCat.Data.Model;

namespace TaskCat.Data.Entity
{
    public class AssetEntity : User
    {
        public string Type { get; set; }
        
    }
}
