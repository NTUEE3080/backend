using PitaPairing.Domain.Post;

namespace PitaPairing.Domain.Application;

public static class ApplicationMapperData
{
    public static Application ToDomain(this ApplicationData data)
    {
        return new Application(data.Id, data.Post.ToPrincipal(), data.ApplierPost.ToPrincipal(),
            data.Status.ToDomain());
    }

    public static ApplicationRes ToResp(this Application a)
    {
        var (guid, postPrincipal, applied, applicationStatus) = a;
        return new ApplicationRes(guid, postPrincipal.ToResp(), applied.ToResp(), applicationStatus.ToResp());
    }
}
