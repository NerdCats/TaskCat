namespace TaskCat.Lib.Job.Builders
{
    using Data.Model;
    using Data.Model.Order;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    internal class DeliveryJobBuilder : JobBuilder
    {
        private DeliveryOrder _order;
        public DeliveryJobBuilder(DeliveryOrder order) : base(order)
        {
            this._order = order;
        }

        public override void BuildTasks()
        {
            _job.Tasks = new List<JobTask>();


        }
    }
}