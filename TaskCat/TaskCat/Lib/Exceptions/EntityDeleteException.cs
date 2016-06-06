namespace TaskCat.Lib.Exceptions
{
    using System;

    public class EntityDeleteException : Exception
    {
        /// <summary>
        /// Base constructor to create a EntityDeleteException with a message
        /// </summary>
        /// <param name="message"></param>
        public EntityDeleteException(string message) : base(message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Initiate a EntityDeleteException with a message generated from entityType
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="identifier"></param>
        public EntityDeleteException(string entityType, string identifier) : base(FormulateMessage(entityType, identifier))
        {

        }

        /// <summary>
        /// Initiate a EntityDeleteException with a message generated from entityType
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="identifier"></param>
        public EntityDeleteException(Type entityType, string identifier) : base(FormulateMessage(entityType, identifier))
        {
        }

        /// <summary>
        /// Initiate EntityDeleteException with an inner exception
        /// </summary>
        /// <param name="description"></param>
        /// <param name="inner"></param>
        public EntityDeleteException(string description, Exception inner) : base(description, inner)
        {
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentNullException(nameof(description));
            if (inner == null) throw new ArgumentNullException("inner");
        }

        private static string FormulateMessage(string entityType, string identifier)
        {
            if (string.IsNullOrWhiteSpace(entityType)) throw new ArgumentNullException(nameof(entityType));
            if (string.IsNullOrWhiteSpace(identifier)) throw new ArgumentNullException(nameof(identifier));

            return string.Concat("Deletion of ", entityType, " with identifier ", identifier, " failed");
        }

        private static string FormulateMessage(Type entityType, string identifier)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            return FormulateMessage(entityType.Name, identifier);
        }
    }
}