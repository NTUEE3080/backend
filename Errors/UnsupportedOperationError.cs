namespace PitaPairing.Errors;

public class UnsupportedOperationError : BaseErrorException
{
    public UnsupportedOperationError(string message, string detail) : base(message, detail, new Dictionary<string, string[]>(),
        StatusCodes.Status400BadRequest)
    {
    }

    public UnsupportedOperationError(string message, Exception? innerException, string detail) : base(message, innerException,
        detail, new Dictionary<string, string[]>(), StatusCodes.Status400BadRequest)
    {
    }
}
