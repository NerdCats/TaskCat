namespace TaskCat.Data.Entity
{
    public class JobActivity : DbEntity
    {
        public string JobId { get; set; }
        public string  Operation { get; set; }
        public ReferenceActivity Reference { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }
    }

    public class ReferenceActivity
    {
        public string Id { get; set; }
        public string EntityType { get; set; }
    }
}
