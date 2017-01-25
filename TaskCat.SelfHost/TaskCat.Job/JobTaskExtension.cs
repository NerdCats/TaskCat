using System;
using System.Linq.Expressions;

namespace TaskCat.Job
{
    /// <summary>
    /// JobTaskExtension provides extended decision making capabilities to jobs
    /// based on certain job task conditions.
    /// </summary>
    public class JobTaskExtension
    {
        /// <summary>
        /// Type of the Order/Job this extension is attached to
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// Type of the job task this extension is registered against
        /// </summary>
        public string JobTaskType { get; set; }

        /// <summary>
        /// Condition expression for the exntension to be activated
        /// </summary>
        public Expression<Func<Data.Model.JobTask, bool>> ConditionExpression { get; set; }

        /// <summary>
        /// Execute the extension works. Expects a Func with the respected job input and respected job output
        /// </summary>       
        public Func<Data.Entity.Job, Data.Entity.Job> ExecuteExtension;

        public JobTaskExtension(
            string orderType,
            string jobTaskType,
            Expression<Func<Data.Model.JobTask, bool>> conditionExpression)
        {
            if (string.IsNullOrWhiteSpace(orderType))
                throw new ArgumentException(nameof(orderType));
            if (string.IsNullOrWhiteSpace(jobTaskType))
                throw new ArgumentException(nameof(jobTaskType));
            if (conditionExpression == null)
                throw new ArgumentNullException(nameof(conditionExpression));

            this.OrderType = orderType;
            this.JobTaskType = jobTaskType;
            this.ConditionExpression = conditionExpression;
        }
    }
}
