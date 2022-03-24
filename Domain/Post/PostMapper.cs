using PitaPairing.Domain.Application;
using PitaPairing.Domain.Index;
using PitaPairing.Domain.Module;
using PitaPairing.User;

namespace PitaPairing.Domain.Post;

public static class PostDataMapper
{
    public static PostProps ToProps(this PostData data)
    {
        return new PostProps(data.User.ToPrincipal(), data.Module.ToPrincipal(), data.Index.ToPrincipal(),
            data.LookingFor?.Select(x => x.ToPrincipal()) ?? new List<IndexPrincipal>(),
            data.Completed
        );
    }


    public static PostPrincipal ToPrincipal(this PostData data)
    {
        return new PostPrincipal(data.Id, data.ToProps());
    }
    public static TradePrincipal ToTradePrincipal(this PostData data, byte status)
    {
        return new TradePrincipal(data.ToPrincipal(), status.ToDomain());
    }
    public static Post ToDomain(this PostData data)
    {
        var offers = data.Offers.Where(x => !x.ApplierPost.Completed)
            .Select(x => x.ApplierPost.ToTradePrincipal(x.Status));
        var applications = data.Applications.Where(x => !x.ApplierPost.Completed)
            .Select(x => x.Post.ToTradePrincipal(x.Status));

        return new Post(data.ToPrincipal(), offers, applications);
    }
}

public static class PostWebMapper
{
    public static PostData ToData(this CreatePostReq req, Guid userId)
    {
        var (modulesId, indexId, _) = req;
        return new PostData()
        {
            Completed = false,
            IndexId = indexId,
            ModuleId = modulesId,
            UserId = userId,
        };
    }
    public static TradePrincipalRes ToResp(this TradePrincipal p)
    {
        var ((guid, (owner, modulePrincipal, indexPrincipal, lookingFor, completed)), status) = p;
        return new TradePrincipalRes(guid, owner.ToResp(), indexPrincipal.ToResp(), modulePrincipal.ToResp(),
            lookingFor?.Select(x => x.ToResp()) ?? new List<IndexPrincipalRes>(), status.ToResp());
    }
    public static PostPrincipalResp ToResp(this PostPrincipal p)
    {
        var (guid, (owner, modulePrincipal, indexPrincipal, lookingFor, completed)) = p;
        return new PostPrincipalResp(guid, owner.ToResp(), indexPrincipal.ToResp(), modulePrincipal.ToResp(),
            lookingFor?.Select(x => x.ToResp()) ?? new List<IndexPrincipalRes>(), completed);
    }

    public static PostResp ToResp(this Post p)
    {
        var (postPrincipal, offers, application) = p;

        return new PostResp(postPrincipal.ToResp(), offers.Select(x => x.ToResp()),
            application.Select(x => x.ToResp()));
    }
}
