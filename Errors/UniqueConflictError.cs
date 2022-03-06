using System.Net;

namespace PitaPairing.Errors;

/// <summary>
/// There is a conflicting field in a unique column
/// </summary>
public class UniqueConflictError : BaseErrorException
{
    public UniqueConflictError(string message, string detail) : base(message, detail,
        new Dictionary<string, string[]>(), StatusCodes.Status409Conflict)
    {
    }

    public UniqueConflictError(string message, string detail, Dictionary<string, string[]> additionalInfo) : base(
        message, detail, additionalInfo, StatusCodes.Status409Conflict)
    {
    }

    public UniqueConflictError(string message, Exception? innerException, string detail,
        Dictionary<string, string[]> additionalInfo) : base(message, innerException, detail, additionalInfo,
        StatusCodes.Status409Conflict)
    {
    }

    public UniqueConflictError AddField(string field, params string[] fieldDetails)
    {
        this.AdditionalInfo[field] = fieldDetails;
        return this;
    }
}