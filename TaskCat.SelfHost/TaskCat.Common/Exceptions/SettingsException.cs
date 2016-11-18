namespace TaskCat.Common.Exceptions
{
    using System;
    public class SettingsException<T> : Exception
    {
        public SettingsException(string message) : base(message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
        }

        public SettingsException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
