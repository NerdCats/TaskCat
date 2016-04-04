namespace TaskCat.Data.Model.JobTasks
{
    using Model;
    using System;
    using Result;

    public abstract class AssignAssetTask : JobTask
    {
        public Location From { get; set; }
        public Location To { get; set; }
        public AssignAssetTask(string type, string name, Location from, Location to) : base(type, name)
        {
            From = from;
            To = to;
        }

        public override void UpdateTask()
        {
            //FIXME: I really should use some attribute here to do this
            //this is just plain ghetto
            IsReadytoMoveToNextTask = (From != null && To != null && Asset != null) ? true : false;
            MoveToNextState();
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
