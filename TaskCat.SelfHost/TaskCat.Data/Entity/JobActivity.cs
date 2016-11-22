namespace TaskCat.Data.Entity
{
    using Model.Identity.Profile;
    using Model.Identity.Response;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    public class JobActivity : DbEntity
    {
        public string JobId { get; set; }
        public string Operation { get; set; }

        [BsonIgnoreIfNull]
        public string Value { get; set; }
        [BsonIgnoreIfNull]
        public string Path { get; set; }

        [BsonIgnoreIfNull]
        public ReferenceActivity Reference { get; set; }

        public ReferenceUser User { get; set; }
        public DateTime TimeStamp { get; set; }

        public JobActivity()
        {

        }

        public JobActivity(string jobId, string operation, UserModel userModel)
        {
            this.JobId = jobId;
            this.Operation = operation;
            TimeStamp = DateTime.UtcNow;

            this.User = new ReferenceUser();
            User.Id = userModel.UserId;
            User.Username = userModel.UserName;

            if (userModel.Profile is UserProfile)
                User.Name = (userModel.Profile as UserProfile).FullName;
            else if (userModel.Profile is EnterpriseUserProfile)
                User.Name = (userModel.Profile as EnterpriseUserProfile).CompanyName;
            else if (userModel.Profile is AssetProfile)
                User.Name = (userModel.Profile as AssetProfile).FullName;
        }
    }

    public class ReferenceActivity
    {
        public string Id { get; set; }
        public string EntityType { get; set; }

        public ReferenceActivity()
        { }

        public ReferenceActivity(string id, string entityType)
        {
            this.Id = id;
            this.EntityType = entityType;
        }
    }

    public class ReferenceUser
    {
        public string Username { get; set; }
        public string Id { get; set; }

        [BsonIgnoreIfNull]
        public string Name { get; set; }
    }

    public class JobActivityOperatioNames
    {
        public const string Add = "Add";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }
}
