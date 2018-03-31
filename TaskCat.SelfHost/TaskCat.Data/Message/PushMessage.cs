namespace TaskCat.Data.Message
{
    public class PushMessage
    {
        public string ReferenceId { get; set; }
        public Entity.Job Job { get; set; }
        public string JobActivityOperationName { get; set; }
    }
}
