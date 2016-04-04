namespace TaskCat.Data.Model.JobTasks
{
    using Model;

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
    }
}
