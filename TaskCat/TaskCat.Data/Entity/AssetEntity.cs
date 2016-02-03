using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskCat.Data.Entity;
using TaskCat.Data.Entity.Identity;
using TaskCat.Data.Model;
using TaskCat.Data.Model.Identity;

namespace TaskCat.Data.Entity
{
    public class AssetEntity : User
    {
        public AssetEntity(UserModel model) : base(model)
        {

        }
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportedUsers AssetType { get; set; }
        
    }
}
