namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Entity;
    using Data.Model.Api;

    public interface IJobRepository
    {
        Task<Job> GetJob(string id);
        Task<Job> PostJob(JobModel model);
    }
}
