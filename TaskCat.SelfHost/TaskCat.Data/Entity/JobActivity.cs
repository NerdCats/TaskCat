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

        public ReferenceUser ForUser { get; set; }
        public ReferenceUser ByUser { get; set; }
        public DateTime TimeStamp { get; set; }

        public JobActivity()
        {

        }

        public JobActivity(string jobId, string operation, UserModel forUser, ReferenceUser byUser)
        {
            this.JobId = jobId;
            this.Operation = operation;
            TimeStamp = DateTime.UtcNow;

            this.ForUser = new ReferenceUser();
            ForUser.Id = forUser.UserId;
            ForUser.Username = forUser.UserName;

            if (forUser.Profile is UserProfile)
                ForUser.Name = (forUser.Profile as UserProfile).FullName;
            else if (forUser.Profile is EnterpriseUserProfile)
                ForUser.Name = (forUser.Profile as EnterpriseUserProfile).CompanyName;
            else if (forUser.Profile is AssetProfile)
                ForUser.Name = (forUser.Profile as AssetProfile).FullName;

            this.ByUser = byUser;
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

        public ReferenceUser()
        {

        }

        public ReferenceUser(string userId, string userName)
        {
            this.Id = userId;
            this.Username = userName;
        }
    }

    public class JobActivityOperatioNames
    {
        public const string Add = "Add";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }
}
