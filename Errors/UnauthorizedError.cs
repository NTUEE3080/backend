namespace PitaPairing.Errors;

public class UnauthorizedError: BaseErrorException
{
    public UnauthorizedError(string message, string detail) : base(message, detail, new Dictionary<string, string[]>(),
        StatusCodes.Status404NotFound)
    {
    }

    public UnauthorizedError(string message, Exception? innerException, string detail) : base(message, innerException,
        detail, new Dictionary<string, string[]>(), StatusCodes.Status404NotFound)
    {
    }
}