namespace TaskCat.Model.Job
{
    using System.ComponentModel.DataAnnotations;

    public class JobCancellationRequest
    {
        [Required]
        public string JobId { get; set; }
        public string Reason { get; set; }
    }
}