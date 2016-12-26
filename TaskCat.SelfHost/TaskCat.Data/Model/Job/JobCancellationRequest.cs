namespace TaskCat.Data.Model.Job
{
    using System.ComponentModel.DataAnnotations;

    public class JobCancellationRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "JobId not provided")]
        public string JobId { get; set; }
        public string Reason { get; set; }
    }
}