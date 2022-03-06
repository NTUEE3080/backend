namespace PitaPairing.Errors;

public class NoPermissionError : BaseErrorException
{
    public NoPermissionError(string message, string detail) : base(message, detail, new Dictionary<string, string[]>(),
        StatusCodes.Status403Forbidden)
    {
    }

    public NoPermissionError(string message, Exception? innerException, string detail) : base(message, innerException,
        detail, new Dictionary<string, string[]>(), StatusCodes.Status403Forbidden)
    {
    }
}