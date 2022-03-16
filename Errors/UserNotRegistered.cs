namespace PitaPairing.Errors;

public class UserNotRegisteredError : BaseErrorException
{
    public UserNotRegisteredError(string message, string detail) : base(message, detail, new Dictionary<string, string[]>(), StatusCodes.Status401Unauthorized)
    {
    }

    public UserNotRegisteredError(string message, Exception? innerException, string detail) : base(message, innerException, detail, new Dictionary<string, string[]>(), StatusCodes.Status401Unauthorized)
    {
    }
}
