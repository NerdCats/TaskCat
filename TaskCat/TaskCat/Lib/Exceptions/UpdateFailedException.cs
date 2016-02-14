namespace TaskCat.Lib.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UpdateFailedException : Exception
    {
        public string PropertyName { get; private set; }
        public UpdateFailedException(string property) : base("Update Failed of "+property)
        {
            this.PropertyName = property;
        }
    }
}