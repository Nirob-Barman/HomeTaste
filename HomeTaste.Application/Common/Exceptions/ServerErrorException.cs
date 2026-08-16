namespace HomeTaste.Application.Common.Exceptions
{
    /// <summary>
    /// For deliberate 500 responses where the message is meant to reach the client - the
    /// replacement for the old ResultType.Failure path (which always surfaced result.Message).
    /// Distinct from an unhandled/unexpected exception, which stays generic for the client and
    /// only logs its real message server-side.
    /// </summary>
    public class ServerErrorException : Exception
    {
        public ServerErrorException(string message) : base(message) { }
    }
}
