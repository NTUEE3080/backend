using PitaPairing.Domain.Application;
using PitaPairing.Domain.Index;
using PitaPairing.Domain.Module;
using PitaPairing.User;

namespace PitaPairing.Domain.Post;

public static class PostDataMapper
{
  public static PostProps ToProps(this PostData data)
  {
    return new PostProps(data.Module.ToPrincipal(), data.Index.ToPrincipal(),
      data.LookingFor?.Select(x => x.ToPrincipal()) ?? new List<IndexPrincipal>(),
      data.Completed
    );
  }

  public static PostPrincipal ToPrincipal(this PostData data)
  {
    return new PostPrincipal(data.Id, data.ToProps());
  }

  public static Post ToDomain(this PostData data)
  {
    return new Post(data.User.ToPrincipal(), data.ToPrincipal(),
      data.Applications?.Select(x => x.ToPrincipal()) ?? new List<ApplicationPrincipal>());
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

  public static PostPrincipalResp ToResp(this PostPrincipal p)
  {
    var (guid, (modulePrincipal, indexPrincipal, lookingFor, completed)) = p;
    return new PostPrincipalResp(guid, indexPrincipal.ToResp(), modulePrincipal.ToResp(), completed,
      lookingFor?.Select(x => x.ToResp()) ?? new List<IndexPrincipalRes>());
  }

  public static PostResp ToResp(this Post p)
  {
    var (userPrincipal, postPrincipal, applicationPrincipals) = p;

    return new PostResp(userPrincipal.ToResp(),
      postPrincipal.ToResp(),
      applicationPrincipals?.Select(x => x.ToResp()) ?? new List<ApplicationPrincipalRes>()
    );
  }
}