namespace TaskCat.Data.Model.JobTasks.Preference
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class JobTaskETAPreference
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "JobTask type is not defined while defining ETA preference")]
        public string Type { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "ETA Not defined while defining ETA preference")]
        public DateTime? ETA { get; set; }
    }
}
