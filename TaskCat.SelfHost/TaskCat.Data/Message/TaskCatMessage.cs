namespace TaskCat.Data.Message
{
    public class TaskCatMessage
    {
        public string ReferenceId { get; set; }
        public string JobID { get; set; }
        public string JobHRID { get; set; }
        public string RemoteJobStage { get; set; }
        public string RemoteJobState { get; set; }
    }
}
