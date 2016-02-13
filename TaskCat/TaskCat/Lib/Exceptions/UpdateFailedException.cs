using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskCat.Lib.Exceptions
{
    public class UpdateFailedException : Exception
    {
        public string PropertyName { get; private set; }
        public UpdateFailedException(string property) : base("Update Failed of "+property)
        {
            this.PropertyName = property;
        }
    }
}