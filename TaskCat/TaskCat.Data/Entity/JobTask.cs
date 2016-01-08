namespace TaskCat.Data.Entity
{
    using JobTasks;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [BsonKnownTypes(typeof(FetchRideTask))]
    public abstract class JobTask : DbEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public JobTaskStates State { get; set; }
        public Asset AssignedAsset { get; set; }
        public DateTime ETA { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public JobTask()
        {
            
        }

        public JobTask(string type)
        {
            Type = type;
        }

    }
}
