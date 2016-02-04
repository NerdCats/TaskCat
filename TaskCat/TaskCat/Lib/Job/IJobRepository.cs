namespace TaskCat.Lib.Job
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TaskCat.Data.Entity;
    using Data.Model.Api;
    using Data.Model.Pagination;

    public interface IJobRepository
    {
        Task<JobEntity> GetJob(string id);
        Task<JobEntity> PostJob(JobModel model);
        Task<IEnumerable<JobEntity>> GetJobs(string type, int start, int limit);
        Task<PageEnvelope<JobEntity>> GetJobsEnveloped(string type, int start, int limit);
    }
}
