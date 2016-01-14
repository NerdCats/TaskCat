namespace TaskCat.Data.Model.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class JobModel
    {
        public string Name { get; set; }
        public List<JobTask> Tasks { get; set; }

    }
}
