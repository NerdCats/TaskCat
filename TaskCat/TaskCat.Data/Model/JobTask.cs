namespace TaskCat.Data.Model
{
    using Entity;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class JobTask
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public JobTaskStates State { get; set; }
        public AssetEntity AssignedAsset { get; set; }
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

        public JobTask(string type, string name):this(type)
        {
            this.Name = name;
        }

    }
}
