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
        public object Value { get; set; }
        [BsonIgnoreIfNull]
        public string Path { get; set; }

        [BsonIgnoreIfNull]
        public ReferenceActivity Reference { get; set; }

        public ReferenceUser ForUser { get; set; }
        public ReferenceUser ByUser { get; set; }
        public DateTime TimeStamp { get; set; }

        public string ActionText { get; set; }

        public JobActivity()
        { }

        public JobActivity(Job job, string operation, ReferenceUser byUser)
        {
            InitiateActivity(job, operation);
                      
            this.ByUser = byUser;
        }

        public JobActivity(Job job, string operation)
        {
            InitiateActivity(job, operation);

            this.ByUser = new ReferenceUser(job.JobServedBy);
        }

        private void InitiateActivity(Job job, string operation)
        {
            this.JobId = job.Id;
            this.Operation = operation;
            TimeStamp = DateTime.UtcNow;
            this.ForUser = new ReferenceUser(job.User);
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

        [BsonIgnore]
        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(Name) ?
                    Username : Name;
            }
        }

        public ReferenceUser()
        { }

        public ReferenceUser(string userId, string userName)
        {
            this.Id = userId;
            this.Username = userName;
        }

        public ReferenceUser(UserModel userModel)
        {
            Id = userModel.UserId;
            Username = userModel.UserName;

            if (userModel.Profile is AssetProfile)
                Name = (userModel.Profile as AssetProfile).FullName;
            else if (userModel.Profile is UserProfile)
                Name = (userModel.Profile as UserProfile).FullName;
            else if (userModel.Profile is EnterpriseUserProfile)
                Name = (userModel.Profile as EnterpriseUserProfile).CompanyName;
        }
    }

    public class JobActivityOperatioNames
    {
        public const string Add = "Add";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Claim = "Claim";
    }
}
