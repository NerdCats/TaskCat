namespace TaskCat.Model.JobTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data.Entity;
    using TaskCat.Data.Model;
    using Data.Model.Identity.Response;

    public class DeliveryTask : JobTask
    {
        public Location From { get; set; }
        public Location To { get; set; }
        public AssetModel Asset { get; set; }


        public DeliveryTask()
        {
           
        }

        public override void SetPredecessor(JobTask task, bool validateDependency = true)
        {
            base.SetPredecessor(task, validateDependency); 
            if(validateDependency)
            {
                //Validate here
                
            }
             
        }

        public override void UpdateTask()
        {
            throw new NotImplementedException();
        }
    }
}