using PitaPairing.Domain.Index;
using PitaPairing.Domain.Post;
using PitaPairing.User;

namespace PitaPairing.Domain.Application;

public enum ApplicationStatus
{
    Accepted = 0,
    Rejected = 1,
    Pending = 2,
}

public class ApplicationStatusConverter
{
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

public record ApplicationProps(
    Guid PostId,
    ApplicationStatus Accepted,
    UserPrincipal User,
    IEnumerable<IndexPrincipal> Offer
);

public record ApplicationPrincipal(
    Guid Id, ApplicationProps Props);

public record Application(
    ApplicationPrincipal Principal,
    PostPrincipal Post);

public record ApplicationPrincipalRes(Guid Id, Guid PostId, string Status, UserPrincipalResp User,
    IEnumerable<IndexPrincipalRes> Offers);

public record ApplicationRes(ApplicationPrincipalRes Principal, PostPrincipalResp Post);

public record CreateApplicationReq(IEnumerable<Guid> OfferId);
