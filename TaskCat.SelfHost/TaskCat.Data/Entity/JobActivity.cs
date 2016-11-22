namespace TaskCat.Data.Entity
{
    using System;
    public class JobActivity : DbEntity
    {
        public string JobId { get; set; }
        public string Operation { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }

        public ReferenceActivity Reference { get; set; }
        public ReferenceUser User { get; set; }

        public DateTime TimeStamp { get; set; }

        public JobActivity()
        {
            TimeStamp = DateTime.UtcNow;
        }
    }

    public class ReferenceActivity
    {
        public string Id { get; set; }
        public string EntityType { get; set; }
    }

    public class ReferenceUser
    {
        public string Username { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class JobActivityOperatioNames
    {
        public const string Add = "Add";
        public const string Update = "Update";
        public const string Delete = "Delete";
    }
}
