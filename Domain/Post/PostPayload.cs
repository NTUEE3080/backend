using PitaPairing.Domain.Application;
using PitaPairing.Domain.Index;
using PitaPairing.Domain.Module;
using PitaPairing.User;

namespace PitaPairing.Domain.Post;



public record CreatePostReq(
    Guid ModulesId,
    Guid IndexId,
    IEnumerable<Guid> LookingForId);

public record PostPrincipalResp(
    Guid Id,
    Guid OwnerId,
    IndexPrincipalRes Index,
    ModulePrincipalRes Module,
    bool Completed,
    IEnumerable<IndexPrincipalRes> LookingFor
);

public record PostResp(
    UserPrincipalResp User,
    PostPrincipalResp Post,
    IEnumerable<ApplicationPrincipalRes> Applications);
