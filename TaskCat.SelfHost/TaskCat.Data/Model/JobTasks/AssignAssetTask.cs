﻿namespace TaskCat.Data.Model.JobTasks
{
    using Model;
    using System;
    using Result;
    using Geocoding;

    public abstract class AssignAssetTask : JobTask
    {
        public DefaultAddress From { get; set; }
        public DefaultAddress To { get; set; }

        public AssignAssetTask(string type, DefaultAddress from, DefaultAddress to) : base(type)
        {
            From = from;
            To = to;
        }

        public override void UpdateTask()
        {
            //FIXME: I really should use some attribute here to do this
            //this is just plain ghetto
            IsReadytoMoveToNextTask = (From != null && To != null && Asset != null) ? true : false;
            UpdateStateParams();
        }

        public override JobTaskResult SetResultToNextState()
        {
            var result = new DefaultAssignAssetTaskResult();
            result.ResultType = typeof(DefaultAssignAssetTaskResult);
            if (this.Asset == null)
                throw new InvalidOperationException("Moving to next state when Asset is null.");
            result.Asset = this.Asset;
            result.From = this.From;
            result.TaskCompletionTime = DateTime.UtcNow;
            result.To = this.To;

            return result;
        }
    }
}
