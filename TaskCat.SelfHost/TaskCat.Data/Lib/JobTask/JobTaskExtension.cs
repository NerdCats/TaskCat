namespace TaskCat.Data.Lib.JobTask
{
    using System;
    using System.Linq.Expressions;
    using Entity;

    /// <summary>
    /// JobTaskExtension provides extended decision making capabilities to jobs
    /// based on certain job task conditions.
    /// </summary>
    public abstract class JobTaskExtension
    {
        /// <summary>
        /// Type of the JobTask this extension is attached to
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Condition expression for the exntension to be activated
        /// </summary>
        public Expression<Func<Model.JobTask, bool>> ConditionExpression { get; set; }

        /// <summary>
        /// Execute the extension works
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public abstract Job ExecuteExtension(Job job);
    }
}
