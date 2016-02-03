namespace TaskCat.Data.Entity
{
    using MongoDB.Bson.Serialization.Attributes;
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

    public class AssetEntity
    {
        //FIXME: Need a simplified vehicleInfo class here 
        public string UserRef { get; set; }
        public double AverageRating { get; set; }

        public AssetEntity(string userRef)
        {
            this.UserRef = userRef;
        }
    }
}
