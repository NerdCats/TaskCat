namespace TaskCat.Lib.Utility
{
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public static class SettingsHelper
    {
        public static void Validate<T>(this T settingsClass)
        {
            if (settingsClass == null)
            {
                throw new SettingsException<T>($"Invalid and empty {settingsClass.GetType().Name}", new ArgumentNullException(nameof(settingsClass)));
            }
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(settingsClass, new ValidationContext(settingsClass), validationResults, true);

            if (validationResults.Count > 0)
                throw new SettingsException<T>(string.Join(",", validationResults.Select(x => x.ErrorMessage)));
        }
    }
}