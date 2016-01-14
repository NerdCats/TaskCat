namespace TaskCat.Lib.Job.Builders
{
    using Data.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using TaskCat.Data.Entity;
    public class RideJobBuilder : JobBuilder
    {
        public RideJobBuilder(string name) : base(name)
        {

        }

        public override void BuildTaks()
        { 
            _job.Tasks = new List<JobTask>();
            
            //Now we would add FetchRideTask
            //PickupTask
            //DropPassengerTask here :)
        }


    }
}