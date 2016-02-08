namespace TaskCat.Data.Entity
{
    using AspNet.Identity.MongoDB;
    using MongoDB.Bson;
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
    using Model.Identity.Registration;
    using Model.Identity.Profile;

    public class Asset :User
    {
        public double AverageRating { get; set; } = 0.0;
        public Asset(AssetRegistrationModel model, AssetProfile profile) : base(model, profile, RoleNames.ROLE_ASSET)
        {
        }     
    }
}
