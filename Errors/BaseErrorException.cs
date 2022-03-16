using System.Net;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace PitaPairing.Errors;

public abstract class BaseErrorException : Exception
{
    public BaseErrorException(string message, string detail, Dictionary<string, string[]> additionalInfo,
        int statusCode)
        : base(message)
    {
        Detail = detail;
        AdditionalInfo = additionalInfo;
        StatusCode = statusCode;
    }

    public BaseErrorException(string message, Exception? innerException, string detail,
        Dictionary<string, string[]> additionalInfo, int statusCode) : base(message, innerException)
    {
        Detail = detail;
        AdditionalInfo = additionalInfo;
        StatusCode = statusCode;
    }

    public string Detail { get; }
    public int StatusCode { get; }
    public Dictionary<string, string[]> AdditionalInfo { get; }

    public BaseErrorException AddInfo(string field, params string[] fieldDetails)
    {
        this.AdditionalInfo[field] = fieldDetails;
        return this;
    }

}

public static class ErrorMiddleware
{
    public static Action<ProblemDetailsOptions> ConfigureProblemDetails(IWebHostEnvironment e)
    {
        return (setup) =>
        {
            setup.IncludeExceptionDetails = (_, _) => e.IsDevelopment();
            setup.Map<BaseErrorException>(BaseErrorExceptionMapper);
        };
    }

    public static ProblemDetails BaseErrorExceptionMapper(HttpContext ctx, BaseErrorException ex)
    {
        return new HttpValidationProblemDetails(ex.AdditionalInfo)
        {
            Title = ex.Message,
            Detail = ex.Detail,
            Type = ex.GetType().ToString(),
            Instance = ctx.Request.Path,
            Status = ex.StatusCode,
        };
    }
}
