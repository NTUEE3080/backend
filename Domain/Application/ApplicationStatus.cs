using Serilog;

namespace PitaPairing.Domain.Application;

public enum ApplicationStatus
{
    Accepted = 0,
    Rejected = 1,
    Pending = 2,
}

public static class ApplicationStatusConverter
{
    public static ApplicationStatus ToDomain(this byte status)
    {
        return status switch
        {
            0 => ApplicationStatus.Accepted,
            1 => ApplicationStatus.Rejected,
            2 => ApplicationStatus.Pending,
            _ => ((Func<ApplicationStatus>)(() =>
           {
               Log.Error("No such Application Status {@Status}", status);
               throw new KeyNotFoundException($"No such Application status: {status}");
           }))()
        };
    }

    public static byte ToData(this ApplicationStatus status)
    {
        return (byte)status;
    }

    public static string ToResp(this ApplicationStatus status)
    {
        return EnumToString[status];
    }

    public static ApplicationStatus ToDomain(this string status)
    {
        return StringToEnum[status];
    }


    public static Dictionary<ApplicationStatus, string> EnumToString = new Dictionary<ApplicationStatus, string>()
    {
        {ApplicationStatus.Accepted, "accepted"},
        {ApplicationStatus.Rejected, "rejected"},
        {ApplicationStatus.Pending, "pending"},
    };

    public static Dictionary<string, ApplicationStatus> StringToEnum = new Dictionary<string, ApplicationStatus>()
    {
        {"accepted", ApplicationStatus.Accepted},
        {"rejected", ApplicationStatus.Rejected},
        {"pending", ApplicationStatus.Pending},
    };
}
