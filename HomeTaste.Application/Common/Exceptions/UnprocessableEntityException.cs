namespace HomeTaste.Application.Common.Exceptions
{
    /// <summary>
    /// Maps to 422, matching the old (unused-but-defined) ResultType.ValidationFailed case.
    /// </summary>
    public class UnprocessableEntityException : Exception
    {
        public UnprocessableEntityException(string message) : base(message) { }
    }
}
