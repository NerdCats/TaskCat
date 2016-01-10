namespace TaskCat.Data.Model
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Entity;

    public abstract class JobTask 
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public JobTaskStates State { get; set; }

        private string _stateString;
        public virtual string StateString
        {
            get
            {
                //FIXME: This should be done better, may be a generic state string resolver or something?
                switch (State)
                {
                    case JobTaskStates.PENDING:
                        return string.Concat(Name, " is still pending");
                    case JobTaskStates.IN_PROGRESS:
                        return string.Concat(Name, " is in progress");
                    case JobTaskStates.COMPLETED:
                        return string.Concat(Name, " is completed");
                    default:
                        return string.Concat(Name, " is in state ", State.ToString().ToLower());
                }
            }
            set
            {
                _stateString = value;
            }
        }
        public Asset AssignedAsset { get; set; }
        public DateTime ETA { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public JobTask()
        {

        }

        public JobTask(string name, string type)
        {
            Name = name;
            Type = type;
        }



    }
}
