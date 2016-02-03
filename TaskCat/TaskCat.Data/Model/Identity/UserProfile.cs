namespace TaskCat.Data.Model.Identity
{
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public string FullName { get { return string.Concat(FirstName, " ", LastName); } }

        public int Age { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
}
