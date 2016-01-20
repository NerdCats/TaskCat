using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskCat.Data.Model;

namespace TaskCat.Lib.Exceptions
{
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