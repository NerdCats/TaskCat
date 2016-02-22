namespace TaskCat.Lib.Utility.JsonPatch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Data.Model;
    using Marvin.JsonPatch.Adapters;
    using Marvin.JsonPatch.Operations;
    using Marvin.JsonPatch.Helpers;
    using Marvin.JsonPatch.Exceptions;
    using Marvin.JsonPatch;

    internal class JobTaskPatchAdapter : IObjectAdapter<JobTask>
    {

        public void Add(Operation<JobTask> operation, JobTask objectToApplyTo)
        {
            Add(operation.path, operation.value, objectToApplyTo, operation);
        }

        private void Add(string path, object value, JobTask objectToApplyTo, Operation<JobTask> operationToReport)
        {
            var pathResult = PropertyHelpers.GetActualPropertyPath(
                path,
                objectToApplyTo,
                operationToReport,
                true);

            var appendList = pathResult.ExecuteAtEnd;
            var positionAsInteger = pathResult.NumericEnd;
            var actualPathToProperty = pathResult.PathToProperty;

            var pathProperty = PropertyHelpers
                .FindProperty(objectToApplyTo, actualPathToProperty);

            if (pathProperty == null)
            {
                throw new JsonPatchException(
                                new JsonPatchError(
                                    objectToApplyTo,
                                    operationToReport,
                                    string.Format("Patch failed: property at location path: {0} does not exist", path))
                                , 422);
            }

        }

        public void Copy(Operation<JobTask> operation, JobTask objectToApplyTo)
        {
            throw new NotImplementedException();
        }

        public void Move(Operation<JobTask> operation, JobTask objectToApplyTo)
        {
            throw new NotImplementedException();
        }

        public void Remove(Operation<JobTask> operation, JobTask objectToApplyTo)
        {
            throw new NotImplementedException();
        }

        public void Replace(Operation<JobTask> operation, JobTask objectToApplyTo)
        {
            throw new NotImplementedException();
        }

        public void Test(Operation<JobTask> operation, JobTask objectToApplyTo)
        {
            throw new NotImplementedException();
        }
    }
}