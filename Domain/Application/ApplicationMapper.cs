using PitaPairing.Domain.Index;
using PitaPairing.Domain.Post;
using PitaPairing.User;
using Serilog;

namespace PitaPairing.Domain.Application;

public static class ApplicationDataMapper
{
    #region DataToDomain

    public static ApplicationStatus ToDomain(this byte status)
    {
        return status switch
        {
            0 => ApplicationStatus.Accepted,
            1 => ApplicationStatus.Rejected,
            2 => ApplicationStatus.Pending,
            _ => ((Func<ApplicationStatus>) (() =>
            {
                Log.Error("No such Application Status {@Status}", status);
                throw new KeyNotFoundException($"No such Application status: {status}");
            }))()
        };
    }

    public static byte ToData(this ApplicationStatus status)
    {
        return (byte) status;
    }

    public static ApplicationProps ToProps(this ApplicationData d)
    {
        return new ApplicationProps(d.Status.ToDomain(), d.User.ToPrincipal(),
            d.Offers?.Select(x => x.ToPrincipal()) ?? new List<IndexPrincipal>());
    }

    public static ApplicationPrincipal ToPrincipal(this ApplicationData d)
    {
        return new ApplicationPrincipal(d.Id, d.ToProps());
    }

    public static Application ToDomain(this ApplicationData d)
    {
        return new Application(d.ToPrincipal(), d.Post.ToPrincipal());
    }

    #endregion
}

public static class ApplicationWebMapper
{
    // Still need to link Offers
    public static ApplicationData ToData(this CreateApplicationReq r, Guid userId, Guid postId)
    {
        return new ApplicationData(Guid.Empty, userId)
        {
            PostId = postId,
            Status = ApplicationStatus.Pending.ToData(),
        };
    }

    public static ApplicationPrincipalRes ToResp(this ApplicationPrincipal p)
    {
        var (guid, (status, userPrincipal, indexPrincipals)) = p;
        return new ApplicationPrincipalRes(guid,
            ApplicationStatusConverter.EnumToString[status],
            userPrincipal.ToResp(),
            indexPrincipals?.Select(x => x.ToResp()) ?? new List<IndexPrincipalRes>());
    }

    public static ApplicationRes ToResp(this Application a)
    {
        var (applicationPrincipal, postPrincipal) = a;
        return new ApplicationRes(applicationPrincipal.ToResp(), postPrincipal.ToResp());
    }
}