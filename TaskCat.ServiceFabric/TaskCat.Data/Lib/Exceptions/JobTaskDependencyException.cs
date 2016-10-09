namespace TaskCat.Data.Lib.Exceptions
{
    using System;
    using Model;

    public class JobTaskDependencyException : ArgumentException
    {
        public JobTask JobTask { get; set; }
        public string PropertyName { get; set; }

        public JobTaskDependencyException(string message) : base(message)
        {

        }

        public JobTaskDependencyException(string message, JobTask jobTask, Exception innerException) : base(message, innerException)
        {
            JobTask = jobTask;
        }

    }
}