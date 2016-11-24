namespace TaskCat.Data.Entity
{
    using Model.Identity.Profile;
    using Model.Identity.Response;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class JobActivity : DbEntity
    {
        public string JobId { get; set; }
        public string HRID { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        private Job Job;

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

        public JobActivity(Job job, string operation, ReferenceUser byUser, ReferenceActivity referenceActivity = null)
        {
            InitiateActivity(job, operation);

            this.Reference = referenceActivity;
            this.ByUser = byUser;

            if (referenceActivity != null)
                GenerateActionText(this.Job, this.Operation);
        }

        public JobActivity(Job job, string operation, string path, ReferenceUser byUser, ReferenceActivity referenceActivity = null) : this(job, operation, byUser, referenceActivity)
        {
            this.Path = path;
        }

        public JobActivity(Job job, string operation, ReferenceActivity referenceActivity = null)
        {
            InitiateActivity(job, operation);

            this.Reference = referenceActivity;
            this.ByUser = new ReferenceUser(job.JobServedBy);

            if (referenceActivity != null)
                GenerateActionText(this.Job, this.Operation);
        }

        public JobActivity(Job job, string operation, string path, ReferenceActivity referenceActivity = null) : this(job, operation, referenceActivity)
        {
            this.Path = path;
        }

        private void InitiateActivity(Job job, string operation)
        {
            this.JobId = job.Id;
            this.HRID = job.HRID;

            this.Job = job;

            this.Operation = operation;
            TimeStamp = DateTime.UtcNow;
            this.ForUser = new ReferenceUser(job.User);
        }

        private void GenerateActionText(Job job, string operation)
        {
            if (operation == JobActivityOperatioNames.Create)
            {
                this.ActionText = $"{this.ByUser} {operation.ToLower()}d {job.HRID}";
            }
            else if (operation == JobActivityOperatioNames.Claim)
            {
                this.ActionText = $"{this.ByUser} {operation.ToLower()}d {job.HRID}";
            }
            else if (operation == JobActivityOperatioNames.Update)
            {
                if (Reference != null)
                {
                    this.ActionText = $"{this.ByUser} {operation.ToLower()}d {Path.Split('.').Last()} of {Reference.EntityType} of {job.HRID} to {Value}";
                }
                else
                {
                    this.ActionText = $"{this.ByUser} {operation.ToLower()}d {Path} of {job.HRID} to {Value}";
                }
            }
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
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Claim = "Claim";
    }
}
