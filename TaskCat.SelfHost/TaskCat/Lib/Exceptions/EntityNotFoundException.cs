namespace TaskCat.Lib.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Base constructor to create a EntityNotFoundException with a message
        /// </summary>
        /// <param name="message">Exception message that describes what element is missing</param>
        public EntityNotFoundException(string message) : base(message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Initiate a EntityNotFoundException with a message generated from stringified entityType
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="identifier"></param>
        public EntityNotFoundException(string entityType, string identifier) : base(FormulateMessage(entityType, identifier))
        {
        }

        /// <summary>
        /// Initiate a EntityNotFoundException with a message generated from entityType
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="identifier"></param>
        public EntityNotFoundException(Type entityType, string identifier) : base(FormulateMessage(entityType, identifier))
        {
        }

        /// <summary>
        /// Initiate EntityNotFoundException with an inner exception
        /// </summary>
        /// <param name="description"></param>
        /// <param name="inner"></param>
        public EntityNotFoundException(string description, Exception inner) : base(description, inner)
        {
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentNullException(nameof(description));
            if (inner == null) throw new ArgumentNullException("inner");
        }

        /// <summary>
        /// Initiate EntityNotFoundException with serialization info and context
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private static string FormulateMessage(string entityType, string identifier)
        {
            if (string.IsNullOrWhiteSpace(entityType)) throw new ArgumentNullException(nameof(entityType));
            if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentNullException(nameof(identifier));

            return string.Concat(entityType, " with identifier ", identifier, " is not available, invalid");
        }

        private static string FormulateMessage(Type entityType, string identifier)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            return FormulateMessage(entityType.Name, identifier);
        }
    }
}