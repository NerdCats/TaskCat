namespace TaskCat.Lib.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an error with the details described as an ErrorDocument
    /// </summary>
    public class ServerErrorException : Exception
    {
        public ServerErrorException()
        {
        }

        public ServerErrorException(string message) : base(message)
        {
        }

        public ServerErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}