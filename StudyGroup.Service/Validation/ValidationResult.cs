namespace StudyGroup.Service.Validation
{
    /// <summary>
    /// Represents the result of a validation operation.
    /// Contains validation success status and any error messages.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Dictionary to store validation errors by field name.
        /// </summary>
        public Dictionary<string, string> Errors { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Indicates whether the validation passed (no errors).
        /// </summary>
        public bool IsValid => !Errors.Any();

        /// <summary>
        /// Adds a validation error for a specific field.
        /// </summary>
        /// <param name="field">The field name that failed validation</param>
        /// <param name="message">The error message describing the validation failure</param>
        public void AddError(string field, string message)
        {
            if (!Errors.ContainsKey(field))
            {
                Errors[field] = message;
            }
            else
            {
                // If field already has an error, append the new message
                Errors[field] += $"; {message}";
            }
        }

        /// <summary>
        /// Gets all error messages as a single formatted string.
        /// </summary>
        /// <returns>Formatted error messages</returns>
        public string GetErrorsAsString()
        {
            if (IsValid) return string.Empty;

            return string.Join(", ", Errors.Select(e => $"{e.Key}: {e.Value}"));
        }

        /// <summary>
        /// Gets errors formatted for API response.
        /// </summary>
        /// <returns>Dictionary suitable for JSON API responses</returns>
        public object GetApiErrors()
        {
            return new
            {
                IsValid = IsValid,
                Errors = Errors,
                Message = IsValid ? "Validation passed" : "Validation failed",
                ErrorCount = Errors.Count
            };
        }
    }
}