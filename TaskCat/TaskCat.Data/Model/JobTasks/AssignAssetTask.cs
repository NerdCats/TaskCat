namespace TaskCat.Data.Model.JobTasks
{
    using Entity;
    using Lib.Constants;
    using TaskCat.Data.Model;
    using Data.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// I m just proposing this task. I think there should be a dedicated task
    /// for assining assets for Delivery Job
    /// </summary>
    public class AssignAssetTask : JobTask
    {
        public Asset ProposedAsset { get; set; }
        public AssignAssetTask() : base(JobTaskTypes.ASSIGN_ASSET, "Assigning an Asset")
        {

        }
        public AssignAssetTask(Asset asset) : this()
        {
            this.ProposedAsset = asset;
        }
        public override void UpdateTask()
        {
            
        }
    }
}
