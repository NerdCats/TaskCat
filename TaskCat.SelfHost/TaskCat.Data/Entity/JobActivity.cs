namespace TaskCat.Data.Entity
{
    using Model.Identity.Response;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Linq;

    public class JobActivity : DbEntity
    {
        public string JobId { get; set; }
        public string HRID { get; set; }

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

        [BsonIgnore]
        public string ActionText { get { return GenerateActionText(); } }

        public JobActivity()
        { }

        public JobActivity(Job job, string operation, ReferenceUser byUser, ReferenceActivity referenceActivity = null)
        {
            InitiateActivity(job, operation);

            if (byUser == null)
                throw new ArgumentNullException(nameof(byUser));

            this.Reference = referenceActivity;
            this.ByUser = byUser;
        }

        public JobActivity(Job job, string operation, string path, ReferenceUser byUser, ReferenceActivity referenceActivity = null) : this(job, operation, byUser, referenceActivity)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            this.Path = path;
        }

        public JobActivity(Job job, string operation, ReferenceActivity referenceActivity = null)
        {
            InitiateActivity(job, operation);

            this.Reference = referenceActivity;
            this.ByUser = new ReferenceUser(job.JobServedBy);
        }

        public JobActivity(Job job, string operation, string path, ReferenceActivity referenceActivity = null) : this(job, operation, referenceActivity)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            this.Path = path;
        }

        private void InitiateActivity(Job job, string operation)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException(nameof(operation));

            this.JobId = job.Id;
            this.HRID = job.HRID;

            this.Operation = operation;
            TimeStamp = DateTime.UtcNow;
            this.ForUser = new ReferenceUser(job.User);
        }

        private string GenerateActionText()
        {
            if (Operation == JobActivityOperatioNames.Create)
            {
                return $"{this.ByUser.DisplayName} {Operation.ToLower()}d {HRID}";
            }
            else if (Operation == JobActivityOperatioNames.Claim)
            {
                return $"{this.ByUser.DisplayName} {Operation.ToLower()}ed {HRID}";
            }
            else if (Operation == JobActivityOperatioNames.Update)
            {
                if (Reference != null)
                {
                    var prefix = $"{this.ByUser.DisplayName} {Operation.ToLower()}d {Path} of {Reference.EntityType} of {HRID}";
                    return Value!=null? $"{prefix} to {Value}": prefix;
                }
                else
                {
                    var prefix = $"{this.ByUser.DisplayName} {Operation.ToLower()}d {Path} of {HRID}";
                    return Value != null ? $"{prefix} to {Value}" : prefix;
                }
            }
            else if (Operation == JobActivityOperatioNames.Restore || Operation == JobActivityOperatioNames.Cancel)
            {
                return $"{this.ByUser.DisplayName} {Operation.ToLower()}d {HRID}";
            }

            return null;
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
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException(nameof(entityType));

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
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException(nameof(userId));
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(nameof(userName));

            this.Id = userId;
            this.Username = userName;
        }

        public ReferenceUser(UserModel userModel)
        {
            if (userModel == null)
                throw new ArgumentNullException(nameof(userModel));

            Id = userModel.UserId;
            Username = userModel.UserName;
            Name = userModel.Profile?.GetName();
        }
    }

    // Needs to be renamed 
    // WARNING: COMMIT BEFORE RENAMING, OR THIS MAY END THE WORLD
    public class JobActivityOperatioNames
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Claim = "Claim";
        public const string Cancel = "Cancel";
        public const string Restore = "Restore";
    }
}
