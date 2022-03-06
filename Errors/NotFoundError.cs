namespace PitaPairing.Errors;

public class NotFoundError : BaseErrorException
{

    public NotFoundError(string message, string detail) : base(message, detail, new Dictionary<string, string[]>(),
        StatusCodes.Status404NotFound)
    {
    }

    public NotFoundError(string message, Exception? innerException, string detail) : base(message, innerException,
        detail, new Dictionary<string, string[]>(), StatusCodes.Status404NotFound)
    {
    }
}