using FluentValidation.Results;

namespace HomeTaste.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown by the MediatR ValidationBehavior when FluentValidation rules fail.
    /// Maps to the same 400 status the old ResultType.ValidationError produced.
    /// </summary>
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new List<string>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures.Select(f => f.ErrorMessage).ToList();
        }

        public ValidationException(List<string> errors) : this()
        {
            Errors = errors;
        }
    }
}
