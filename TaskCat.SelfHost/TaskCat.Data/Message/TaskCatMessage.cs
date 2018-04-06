namespace TaskCat.Data.Message
{
    public class TaskCatMessage
    {
        public string ReferenceId { get; set; }
        public Entity.Job Job { get; set; }
        public string RemoteJobStage { get; set; }
        public string RemoteJobState { get; set; }
    }
}
