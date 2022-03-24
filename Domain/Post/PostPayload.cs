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
    UserPrincipalResp Owner,
    IndexPrincipalRes Index,
    ModulePrincipalRes Module,
    IEnumerable<IndexPrincipalRes> LookingFor,
    bool Completed
);

public record TradePrincipalRes(
    Guid Id,
    UserPrincipalResp Owner,
    IndexPrincipalRes Index,
    ModulePrincipalRes Module,
    IEnumerable<IndexPrincipalRes> LookingFor,
    string Status
);

public record PostResp(
    PostPrincipalResp Post,
    IEnumerable<TradePrincipalRes> Offers,
    IEnumerable<TradePrincipalRes> Applications);
